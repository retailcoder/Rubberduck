using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Parsing.VBA.DeclarationCaching;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.Extensions;
using Rubberduck.VBEditor.SafeComWrappers;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.Utility;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Rubberduck.ComClientLibrary.CI
{
    public interface IHeadlessImportService
    {
        void ImportSourceFiles(string path);
    }

    public class HeadlessImportException : Exception
    {
        public HeadlessImportException(string message)
            : base(message)
        {
        }
    }

    public class HeadlessImportService : IHeadlessImportService
    {
        private readonly IVBE _vbe;
        private readonly IFileSystem _fileSystem;
        private readonly IParseManager _parseManager;
        private readonly IModuleNameFromFileExtractor _moduleNameFromFileExtractor;
        private readonly IDeclarationFinderProvider _declarationFinderProvider;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IDictionary<ComponentType, List<IRequiredBinaryFilesFromFileNameExtractor>> _binaryFileExtractors;

        private readonly IDictionary<string, ICollection<ComponentType>> _componentTypesForExtension;
        private readonly ICollection<ComponentType> _reimportableComponentTypes;

        private static readonly string[] _sourceFileExtensions = { ".bas", ".cls", ".frm", ".doccls" };

        public HeadlessImportService(
            IVBE vbe,
            IFileSystem fileSystem,
            IParseManager parseManager,
            IModuleNameFromFileExtractor moduleNameFromFileExtractor,
            IProjectsProvider projectsProvider,
            IEnumerable<IRequiredBinaryFilesFromFileNameExtractor> binaryFileExtractors,
            IDeclarationFinderProvider declarationFinderProvider)
        {
            _vbe = vbe;
            _fileSystem = fileSystem;
            _parseManager = parseManager;
            _moduleNameFromFileExtractor = moduleNameFromFileExtractor;
            _projectsProvider = projectsProvider;
            _binaryFileExtractors = BinaryFileExtractors(binaryFileExtractors);
            _declarationFinderProvider = declarationFinderProvider;

            _componentTypesForExtension = ComponentTypeExtensions.ComponentTypesForExtension(_vbe.Kind);
            _reimportableComponentTypes = _componentTypesForExtension.Values
                .SelectMany(componentTypes => componentTypes)
                .Where(componentType => componentType != ComponentType.Document)
                .ToList();
        }

        public void ImportSourceFiles(string path)
        {
            var sourceFiles = DiscoverSourceFiles(path ?? string.Empty);
            if (!sourceFiles.Any())
            {
                throw new HeadlessImportException("No source files found in the specified path.");
            }

            using (var project = TargetProjectFromVbe())
            {
                if (project == null)
                {
                    throw new HeadlessImportException("Could not find an active VBA project.");
                }

                var suspendResult = _parseManager.OnSuspendParser(this, new[] { ParserState.Ready }, () => ImportSourceFiles(sourceFiles, project));
                if (suspendResult.Outcome != SuspensionOutcome.Completed)
                {
                    throw new HeadlessImportException("Failed to suspend parser for import.");
                }
            }
        }

        private void ImportSourceFiles(IEnumerable<string> sourceFiles, IVBProject project)
        {
            var moduleNames = ExtractModuleNames(sourceFiles);

            var finder = _declarationFinderProvider.DeclarationFinder;
            var existingModules = Modules(moduleNames, project.ProjectId, finder);

            var requiredBinaryFiles = RequiredBinaryFiles(sourceFiles);
            var missingBinaries = FilesWithoutRequiredBinaries(requiredBinaryFiles);
            var filesWithoutRequiredBinaryButWithPossibilityToImportToExistingComponent = FilesWithMechanismToImportToExistingComponent(missingBinaries.Keys);
            var filesWithoutRequiredBinariesWithoutBackupSolution = missingBinaries.Keys
                .Where(fileName => !filesWithoutRequiredBinaryButWithPossibilityToImportToExistingComponent.Contains(fileName))
                .ToList();
            if (filesWithoutRequiredBinariesWithoutBackupSolution.Any())
            {
                throw new HeadlessImportException($"The following files are missing required binaries: {string.Join(", ", filesWithoutRequiredBinariesWithoutBackupSolution)}");
            }

            if (!filesWithoutRequiredBinaryButWithPossibilityToImportToExistingComponent
                .All(filename => existingModules.ContainsKey(filename)
                    && HasMatchingFileExtension(filename, existingModules[filename])))
            {
                // TODO log warning? trace? something?
                //throw new HeadlessImportException($"One or more of the following files is missing required binaries: {string.Join(", ", filesWithoutRequiredBinaryButWithPossibilityToImportToExistingComponent)}");
            }

            var modulesToRemoveBeforeImport = ModulesToRemoveBeforeImport(existingModules);
            //Since we want to import into the existing components, we must not remove them.
            foreach (var filename in filesWithoutRequiredBinaryButWithPossibilityToImportToExistingComponent)
            {
                var module = existingModules[filename];
                if (modulesToRemoveBeforeImport.Contains(module))
                {
                    modulesToRemoveBeforeImport.Remove(module);
                }
            }

            var documentFiles = DocumentFiles(moduleNames);
            //We can only insert into existing documents.
            if (!documentFiles.All(filename => existingModules.ContainsKey(filename)
                && HasMatchingFileExtension(filename, existingModules[filename])))
            {
                var missingDocumentComponents = documentFiles.Where(doc => !existingModules.ContainsKey(doc));
                throw new HeadlessImportException($"The following document modules are missing from the host document and cannot be imported: {string.Join(", ", missingDocumentComponents)}");
            }

            //We must not remove component types we cannot reimport. modules.
            var reimportableComponentTypes = _reimportableComponentTypes;
            modulesToRemoveBeforeImport = modulesToRemoveBeforeImport
                .Where(module => reimportableComponentTypes.Contains(module.ComponentType))
                .ToList();

            using (var components = project.VBComponents)
            {
                foreach (var module in modulesToRemoveBeforeImport)
                {
                    var component = _projectsProvider.Component(module);
                    components.Remove(component);
                }

                foreach (var sourceFile in sourceFiles)
                {
                    if (documentFiles.Contains(sourceFile) || filesWithoutRequiredBinaryButWithPossibilityToImportToExistingComponent.Contains(sourceFile))
                    {
                        //We have to dispose the return value.
                        using (components.ImportSourceFile(sourceFile)) { }
                    }
                    else
                    {
                        //We have to dispose the return value.
                        using (components.Import(sourceFile)) { }
                    }
                }
            }

        }

        private ICollection<string> DocumentFiles(Dictionary<string, string> moduleNames)
        {
            return moduleNames
                .Select(kvp => kvp.Key)
                .Where(filename => _fileSystem.Path.GetExtension(filename) != null
                    && _componentTypesForExtension.TryGetValue(_fileSystem.Path.GetExtension(filename), out var componentTypes)
                    && componentTypes.Contains(ComponentType.Document))
                .ToHashSet();
        }

        private ICollection<QualifiedModuleName> ModulesToRemoveBeforeImport(IDictionary<string, QualifiedModuleName> existingModules)
        {
            return _declarationFinderProvider.DeclarationFinder
                .UserDeclarations(DeclarationType.Module)
                .Select(decl => decl.QualifiedModuleName)
                .ToHashSet();
        }
        protected bool HasMatchingFileExtension(string filename, QualifiedModuleName module)
        {
            var fileExtension = _fileSystem.Path.GetExtension(filename);
            return fileExtension != null
                   && _componentTypesForExtension.TryGetValue(fileExtension, out var componentTypes)
                   && componentTypes.Contains(module.ComponentType);
        }

        private ICollection<string> FilesWithMechanismToImportToExistingComponent(ICollection<string> fileNames)
        {
            return fileNames
                .Where(filename => _fileSystem.Path.GetExtension(filename) != null
                    && _componentTypesForExtension.TryGetValue(_fileSystem.Path.GetExtension(filename), out var componentTypes)
                    && componentTypes.All(componentType => ComponentTypesWithImportMechanismToExistingComponent.Contains(componentType)))
                .ToHashSet();
        }

        //For some component types like user forms and documents we have implemented a way to import them into existing components.
        private static ICollection<ComponentType> ComponentTypesWithImportMechanismToExistingComponent { get; }
            = new List<ComponentType> { ComponentType.Document, ComponentType.UserForm };

        private IDictionary<string, ICollection<string>> FilesWithoutRequiredBinaries(Dictionary<string, ICollection<string>> requiredBinaries)
        {
            var filesWithoutBinaries = new Dictionary<string, ICollection<string>>();
            foreach (var (fileName, requiredBinariesForFile) in requiredBinaries)
            {
                var path = _fileSystem.Path.GetDirectoryName(fileName);
                var missingBinaries = requiredBinariesForFile
                    .Where(binaryFileName => !_fileSystem.File.Exists(_fileSystem.Path.Combine(path, binaryFileName)))
                    .ToList();

                if (missingBinaries.Any())
                {
                    filesWithoutBinaries.Add(fileName, missingBinaries);
                }
            }

            return filesWithoutBinaries;
        }

        private Dictionary<string, ICollection<string>> RequiredBinaryFiles(IEnumerable<string> fileNames)
        {
            var requiredBinaryNames = new Dictionary<string, ICollection<string>>();
            foreach (var filename in fileNames)
            {
                if (requiredBinaryNames.ContainsKey(filename))
                {
                    continue;
                }

                var requiredBinaryFiles = RequiredBinaryFiles(filename);
                if (requiredBinaryFiles.Any())
                {
                    requiredBinaryNames.Add(filename, requiredBinaryFiles);
                }
            }

            return requiredBinaryNames;
        }

        private ICollection<string> RequiredBinaryFiles(string filename)
        {
            var extension = _fileSystem.Path.GetExtension(filename);
            if (extension == null || !_componentTypesForExtension.TryGetValue(extension, out var componentTypes))
            {
                return new List<string>();
            }

            foreach (var componentType in componentTypes)
            {
                if (_binaryFileExtractors.TryGetValue(componentType, out var binaryExtractors))
                {
                    return binaryExtractors
                        .SelectMany(binaryExtractor => binaryExtractor.RequiredBinaryFiles(filename, componentType))
                        .ToHashSet();
                }
            }

            return new List<string>();
        }

        private IDictionary<ComponentType, List<IRequiredBinaryFilesFromFileNameExtractor>> BinaryFileExtractors(IEnumerable<IRequiredBinaryFilesFromFileNameExtractor> extractors)
        {
            var dict = new Dictionary<ComponentType, List<IRequiredBinaryFilesFromFileNameExtractor>>();
            foreach (var extractor in extractors)
            {
                foreach (var componentType in extractor.SupportedComponentTypes)
                {
                    if (!dict.ContainsKey(componentType))
                    {
                        dict.Add(componentType, new List<IRequiredBinaryFilesFromFileNameExtractor>());
                    }

                    dict[componentType].Add(extractor);
                }
            }

            return dict;
        }

        private IEnumerable<string> DiscoverSourceFiles(string path)
        {
            foreach (var file in _fileSystem.Directory.EnumerateFiles(path))
            {
                var extension = _fileSystem.Path.GetExtension(file).ToLowerInvariant();
                if (_sourceFileExtensions.Contains(extension))
                {
                    yield return file;
                }
            }
        }

        private Dictionary<string, QualifiedModuleName> Modules(IDictionary<string, string> moduleNames, string projectId, DeclarationFinder finder)
        {
            var modules = new Dictionary<string, QualifiedModuleName>();
            foreach (var kvp in moduleNames)
            {
                var fileName = kvp.Key;
                var moduleName = kvp.Value;
                var module = Module(moduleName, projectId, finder);
                if (module.HasValue)
                {
                    modules.Add(fileName, module.Value);
                }
            }

            return modules;
        }

        private QualifiedModuleName? Module(string moduleName, string projectId, DeclarationFinder finder)
        {
            foreach (var module in finder.AllModules)
            {
                if (module.ProjectId.Equals(projectId)
                    && module.ComponentName.Equals(moduleName))
                {
                    return module;
                }
            }

            return null;
        }

        private Dictionary<string, string> ExtractModuleNames(IEnumerable<string> filenames)
        {
            var moduleNames = new Dictionary<string, string>();
            foreach (var filename in filenames)
            {
                if (moduleNames.ContainsKey(filename))
                {
                    continue;
                }

                var moduleName = _moduleNameFromFileExtractor.ModuleName(filename);
                if (moduleName != null)
                {
                    moduleNames.Add(filename, moduleName);
                }
            }

            if (!moduleNames.GroupBy(kvp => kvp.Value).All(nameGroup => nameGroup.Count() == 1))
            {
                throw new HeadlessImportException("Duplicate component (module) names in the source files.");
            }

            return moduleNames;
        }

        private IVBProject TargetProjectFromVbe()
        {
            if (_vbe.ProjectsCount == 1)
            {
                using (var projects = _vbe.VBProjects)
                {
                    return projects[1];
                }
            }

            var activeProject = _vbe.ActiveVBProject;
            return activeProject != null && !activeProject.IsWrappingNullReference
                ? activeProject
                : null;
        }
    }
}
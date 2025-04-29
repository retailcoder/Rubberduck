using Rubberduck.CodeAnalysis.Inspections;
using Rubberduck.Parsing.UIContext;
using Rubberduck.Parsing.VBA;
using Rubberduck.Resources.Registration;
using Rubberduck.UnitTesting;
using Rubberduck.VBEditor.ComManagement.TypeLibs;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Rubberduck.ComClientLibrary.CI
{
    public interface IHeadlessInspector
    {
        HeadlessInspectionResults RunHeadless();
    }

    public class HeadlessInspector : IHeadlessInspector
    {
        private readonly RubberduckParserState _state;
        private readonly IInspector _inspector;
        private readonly IUiDispatcher _uiDispatcher;

        public HeadlessInspector(RubberduckParserState state, IInspector inspector, IUiDispatcher uiDispatcher)
        {
            _state = state;
            _inspector = inspector;
            _uiDispatcher = uiDispatcher;
        }

        public HeadlessInspectionResults RunHeadless()
        {
            var task = _state.OnParseRequested(this);
            task.Wait();

            if (_state.Status == ParserState.Ready)
            {
                var results = _inspector.FindIssuesAsync(_state, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

                return new HeadlessInspectionResults
                {
                    InspectionResults = results.Select(e => new HeadlessInspectionResult
                    {
                        Description = e.Description,
                        Inspection = e.Inspection.Name,
                        Location = e.QualifiedSelection.Selection.ToString(),
                        ModuleName = e.QualifiedMemberName?.QualifiedModuleName.Name,
                        ProjectName = e.QualifiedMemberName?.QualifiedModuleName.ProjectName
                    }).ToArray()
                };
            }

            return null;
        }
    }


    [
        ComVisible(true),
        Guid(RubberduckGuid.RubberduckCIClassGuid),
        ProgId(RubberduckProgId.RubberduckCIProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IRubberduckCI)),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public class RubberduckCI : IRubberduckCI
    {
        private readonly IHeadlessImportService _importService;
        private readonly IHeadlessInspector _inspector;
        private readonly ITestEngine _testEngine;

        public RubberduckCI(IHeadlessImportService importService, ITestEngine testEngine, IHeadlessInspector inspector)
        {
            _importService = importService;
            _testEngine = testEngine;
            _inspector = inspector;
        }

        public void ImportSourceFiles(string path) => _importService.ImportSourceFiles(path);

        public ITestOutput RunAllTests() => _testEngine.RunHeadless();

        public ICodeInspectionResults RunInspections() => _inspector.RunHeadless();
    }
}
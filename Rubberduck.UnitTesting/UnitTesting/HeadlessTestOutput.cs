using Rubberduck.Resources.Registration;
using Rubberduck.VBEditor.ComManagement.TypeLibs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rubberduck.UnitTesting
{
    [
        ComVisible(true),
        Guid(RubberduckGuid.RubberduckInspectionResultClassGuid),
        ProgId(RubberduckProgId.RubberduckInspectionResultProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(ITestInfo)),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public class HeadlessInspectionResult : ICodeInspectionResult
    {
        public string Inspection { get; set; }

        public string Description { get; set; }

        public string ProjectName { get; set; }

        public string ModuleName { get; set; }

        public string Location { get; set; }
    }

    [
        ComVisible(true),
        Guid(RubberduckGuid.RubberduckInspectionResultsClassGuid),
        ProgId(RubberduckProgId.RubberduckInspectionResultsProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(ITestInfo)),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public class HeadlessInspectionResults : ICodeInspectionResults
    {
        public ICodeInspectionResult[] InspectionResults { get; set; }
    }

    [
        ComVisible(true),
        Guid(RubberduckGuid.RubberduckTestInfoClassGuid),
        ProgId(RubberduckProgId.TestInfoProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(ITestInfo)),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public class HeadlessTestInfo : ITestInfo
    {
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public string TestName { get; set; }

        public string Folder { get; set; }
        public string Category { get; set; }
        public bool IsIgnored { get; set; }

        public long MillisecondsElapsed { get; set; }
        public string Outcome { get; set; }
        public string Message { get; set; }

        public static HeadlessTestInfo For(TestMethod test, TestResult result)
        {
            return new HeadlessTestInfo
            {
                ProjectName = test.Declaration.ProjectName,
                ModuleName = test.Declaration.QualifiedModuleName.Name,
                TestName = test.Declaration.IdentifierName,

                Folder = test.Declaration.ParentDeclaration.CustomFolder,
                Category = test.Category.Name,
                IsIgnored = test.IsIgnored,

                MillisecondsElapsed = result.Duration,
                Outcome = result.Outcome.ToString(),
                Message = result.Output,
            };
        }
    }

    [
        ComVisible(true),
        Guid(RubberduckGuid.RubberduckTestOutputClassGuid),
        ProgId(RubberduckProgId.TestOutputProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(ITestOutput)),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public class HeadlessTestOutput : ITestOutput
    {
        private readonly List<HeadlessTestInfo> _results = new List<HeadlessTestInfo>();
        public ITestInfo[] Results => _results.ToArray();

        private readonly List<string> _logs = new List<string>();
        public string[] Logs => _logs.ToArray();

        public long MillisecondsElapsed { get; set; }

        internal void Add(HeadlessTestInfo testInfo) => _results.Add(testInfo);
        internal void Log(string message) => _logs.Add(message);
    }
}

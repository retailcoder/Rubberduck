using System.Collections.Generic;

namespace Rubberduck.UnitTesting
{
    public class HeadlessTestInfo
    {
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public string TestName { get; set; }

        public string Folder { get; set; }
        public string Category { get; set; }
        public bool IsIgnored { get; set; }

        public long MillisecondsElapsed { get; set; }
        public TestOutcome Outcome { get; set; }
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
                Outcome = result.Outcome,
                Message = result.Output,
            };
        }
    }

    public class HeadlessTestOutput
    {
        private readonly List<HeadlessTestInfo> _results = new List<HeadlessTestInfo>();
        public HeadlessTestInfo[] Results => _results.ToArray();

        private readonly List<string> _logs = new List<string>();
        public string[] Logs => _logs.ToArray();

        public long MillisecondsElapsed { get; set; }

        internal void Add(HeadlessTestInfo testInfo) => _results.Add(testInfo);
        internal void Log(string message) => _logs.Add(message);
    }
}

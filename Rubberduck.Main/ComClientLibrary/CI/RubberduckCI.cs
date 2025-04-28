using Rubberduck.CodeAnalysis.Inspections;
using Rubberduck.CodeAnalysis.Settings;
using Rubberduck.Parsing.VBA;
using Rubberduck.UnitTesting;
using System.Linq;
using System.Threading;

namespace Rubberduck.ComClientLibrary.CI
{
    public interface IHeadlessInspector
    {
        HeadlessCodeInspectionResults RunHeadless();
    }

    public class HeadlessInspector : IHeadlessInspector
    {
        private readonly RubberduckParserState _state;
        private readonly IBlockingParseService _parser;
        private readonly IInspector _inspector;

        public HeadlessInspector(RubberduckParserState state, IBlockingParseService parser, IInspector inspector)
        {
            _state = state;
            _parser = parser;
            _inspector = inspector;
        }

        public HeadlessCodeInspectionResults RunHeadless()
        {
            _parser.Parse();
            var results = _inspector.FindIssuesAsync(_state, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

            return new HeadlessCodeInspectionResults
            {
                InspectionResults = results.ToArray()
            };
        }
    }

    public interface IRubberduckCI
    {
        void ImportSourceFiles(string path);
        HeadlessTestOutput RunAllTests();
        HeadlessCodeInspectionResults RunInspections(ICodeInspectionSettings settings);
    }

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

        public HeadlessTestOutput RunAllTests() => _testEngine.RunHeadless();

        public HeadlessCodeInspectionResults RunInspections(ICodeInspectionSettings settings) => _inspector.RunHeadless();
    }
}
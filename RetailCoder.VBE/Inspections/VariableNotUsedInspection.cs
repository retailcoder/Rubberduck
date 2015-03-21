using System.Collections.Generic;
using Rubberduck.Parsing;

namespace Rubberduck.Inspections
{
    public class VariableNotUsedInspection : IInspection
    {
        public VariableNotUsedInspection()
        {
            Severity = CodeInspectionSeverity.Hint;
        }

        public string Name { get { return InspectionNames.VariableNotUsed_; } }
        public CodeInspectionType InspectionType { get { return CodeInspectionType.CodeQualityIssues; } }
        public CodeInspectionSeverity Severity { get; set; }

        public IEnumerable<CodeInspectionResultBase> GetInspectionResults(VBProjectParseResult parseResult)
        {
            var issues = parseResult.IdentifierUsageInspector.AllUnusedVariables();
            foreach (var issue in issues)
            {
                yield return new VariableNotUsedInspectionResult(string.Format(Name, issue.Context.GetText()), Severity, issue.Context, issue.QualifiedName);
            }
        }
    }
}
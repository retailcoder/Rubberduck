using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Listeners;

namespace Rubberduck.Inspections
{
    public class ImplicitByRefParameterInspection : IInspection
    {
        public ImplicitByRefParameterInspection()
        {
            Severity = CodeInspectionSeverity.Suggestion;
        }

        public string Name { get { return InspectionNames.ImplicitByRef_; } }
        public CodeInspectionType InspectionType { get { return CodeInspectionType.CodeQualityIssues; } }
        public CodeInspectionSeverity Severity { get; set; }

        public IEnumerable<CodeInspectionResultBase> GetInspectionResults(VBProjectParseResult parseResult) 
        {
            foreach (var module in parseResult.ComponentParseResults)
            {
                var procedures = module.ParseTree.GetContexts<ProcedureListener, ParserRuleContext>(new ProcedureListener(module.QualifiedName));
                foreach (var procedure in procedures)
                {
                    var args = GetArguments(procedure);
                    foreach (var arg in args.Where(arg => arg.BYREF() == null && arg.BYVAL() == null && arg.PARAMARRAY() == null))
                    {
                        var context = new QualifiedContext<VBAParser.ArgContext>(module.QualifiedName, arg);
                        yield return new ImplicitByRefParameterInspectionResult(string.Format(Name, arg.ambiguousIdentifier().GetText()), Severity, context);
                    }
                }
            }
        }

        private static readonly IEnumerable<Func<ParserRuleContext, VBAParser.ArgListContext>> Converters =
            new List<Func<ParserRuleContext, VBAParser.ArgListContext>>
            {
                GetSubArgsList,
                GetFunctionArgsList,
                GetPropertyGetArgsList,
                GetPropertyLetArgsList,
                GetPropertySetArgsList
            };

        private IEnumerable<VBAParser.ArgContext> GetArguments(QualifiedContext<ParserRuleContext> procedureContext)
        {
            var argsList = Converters.Select(converter => converter(procedureContext.Context)).FirstOrDefault(args => args != null);
            if (argsList == null)
            {
                return new List<VBAParser.ArgContext>();
            }

            return argsList.arg();
        }

        private static VBAParser.ArgListContext GetSubArgsList(ParserRuleContext procedureContext)
        {
            var context = procedureContext as VBAParser.SubStmtContext;
            return context == null ? null : context.argList();
        }

        private static VBAParser.ArgListContext GetFunctionArgsList(ParserRuleContext procedureContext)
        {
            var context = procedureContext as VBAParser.FunctionStmtContext;
            return context == null ? null : context.argList();
        }

        private static VBAParser.ArgListContext GetPropertyGetArgsList(ParserRuleContext procedureContext)
        {
            var context = procedureContext as VBAParser.PropertyGetStmtContext;
            return context == null ? null : context.argList();
        }

        private static VBAParser.ArgListContext GetPropertyLetArgsList(ParserRuleContext procedureContext)
        {
            var context = procedureContext as VBAParser.PropertyLetStmtContext;
            return context == null ? null : context.argList();
        }

        private static VBAParser.ArgListContext GetPropertySetArgsList(ParserRuleContext procedureContext)
        {
            var context = procedureContext as VBAParser.PropertySetStmtContext;
            return context == null ? null : context.argList();
        }
    }
}
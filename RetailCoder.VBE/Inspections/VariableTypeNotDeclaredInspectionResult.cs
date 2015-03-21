using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Microsoft.Vbe.Interop;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Inspections
{
    public class VariableTypeNotDeclaredInspectionResult : CodeInspectionResultBase
    {
        public VariableTypeNotDeclaredInspectionResult(string inspection, CodeInspectionSeverity type, ParserRuleContext context, QualifiedModuleName qualifiedName)
            : base(inspection, type, qualifiedName, context)
        {
        }

        public override IDictionary<string, Action<VBE>> GetQuickFixes()
        {
            return
                new Dictionary<string, Action<VBE>>
                {
                    {"Declare as explicit Variant", DeclareAsExplicitVariant}
                };
        }

        private void DeclareAsExplicitVariant(VBE vbe)
        {
            var component = FindComponent(vbe);
            if (component == null)
            {
                throw new InvalidOperationException("'" + QualifiedName + "' not found.");
            }

            var codeLine = component.CodeModule.get_Lines(QualifiedSelection.Selection.StartLine, QualifiedSelection.Selection.LineCount);

            // methods return empty string if soft-cast context is null - just concat results:
            string originalInstruction;
            var fix = DeclareExplicitVariant(Context as VBAParser.VariableSubStmtContext, out originalInstruction);

            if (string.IsNullOrEmpty(originalInstruction))
            {
                fix = DeclareExplicitVariant(Context as VBAParser.ConstSubStmtContext, out originalInstruction);
            }
            
            var fixedCodeLine = codeLine.Replace(originalInstruction, fix);
            component.CodeModule.ReplaceLine(QualifiedSelection.Selection.StartLine, fixedCodeLine);
        }

        private string DeclareExplicitVariant(VBAParser.VariableSubStmtContext context, out string instruction)
        {
            if (context == null)
            {
                instruction = null;
                return null;
            }

            instruction = context.GetText();
            return instruction + ' ' + Tokens.As + ' ' + Tokens.Variant;
        }

        private string DeclareExplicitVariant(VBAParser.ConstSubStmtContext context, out string instruction)
        {
            if (context == null)
            {
                instruction = null;
                return null;
            }

            var parent = (VBAParser.ConstStmtContext) context.Parent;
            instruction = parent.GetText();

            var visibilityContext = parent.visibility();
            var visibility = visibilityContext == null ? string.Empty : visibilityContext.GetText() + ' ';

            var result = visibility
                         + Tokens.Const + ' '
                         + context.ambiguousIdentifier().GetText() + ' '
                         + Tokens.As + ' ' + Tokens.Variant + ' '
                         + context.EQ().GetText() + ' '
                         + context.valueStmt().GetText();
            return result;
        }
    }
}
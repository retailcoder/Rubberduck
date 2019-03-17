using System.Collections.Generic;
using System.Linq;
using Rubberduck.Common;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Results;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Resources.Inspections;

namespace Rubberduck.Inspections.Concrete
{
    public sealed class PublicControlFieldAccessInspection : InspectionBase
    {
        public PublicControlFieldAccessInspection(RubberduckParserState state) : base(state) { }

        protected override IEnumerable<IInspectionResult> DoGetInspectionResults()
        {
            var userFormControls = State.DeclarationFinder.DeclarationsWithType(DeclarationType.Control);
            foreach (var usage in userFormControls.SelectMany(control => control.References))
            {
                if (usage.ParentScoping.ParentDeclaration != usage.Declaration.ParentDeclaration)
                {
                    yield return new IdentifierReferenceInspectionResult(this, 
                        string.Format(InspectionResults.PublicControlFieldAccessInspection, usage.Declaration.IdentifierName), 
                        State, usage);
                }
            }
        }
    }
}

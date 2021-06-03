using System.Runtime.InteropServices;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.UI.Controls;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.Utility;

namespace Rubberduck.UI.Command.ComCommands
{
    public class ProjectExplorerFindAllImplementationsCommand : FindAllImplementationsCommand
    {
        private readonly ISelectedDeclarationProvider _selectedDeclarationProvider;

        public ProjectExplorerFindAllImplementationsCommand(
            IParserStatusProvider parserStatusProvider,
            ISelectedDeclarationProvider selectedDeclarationProvider,
            ISearchResultsWindowViewModel viewModel,
            FindAllImplementationsService finder,
            IVbeEvents vbeEvents)
            : base(parserStatusProvider, selectedDeclarationProvider, viewModel, finder, vbeEvents)
        {
            _selectedDeclarationProvider = selectedDeclarationProvider;
        }

        protected override Declaration FindTarget(object parameter)
        {
            if (parameter is Declaration declaration)
            {
                return declaration;
            }

            return _selectedDeclarationProvider.SelectedProjectExplorerModule();
        }
    }

    /// <summary>
    /// A command that finds all implementations of a specified method, or of the active interface module.
    /// </summary>
    public class FindAllImplementationsCommand : ComCommandBase
    {
        private readonly ISelectedDeclarationProvider _selectedDeclarationProvider;
        private readonly IParserStatusProvider _parserStatusProvider;
        private readonly FindAllImplementationsService _finder;

        public FindAllImplementationsCommand(
            IParserStatusProvider parserStatusProvider,
            ISelectedDeclarationProvider selectedDeclarationProvider, 
            ISearchResultsWindowViewModel viewModel, 
            FindAllImplementationsService finder, 
            IVbeEvents vbeEvents)
            : base(vbeEvents)
        {
            _finder = finder;
            _selectedDeclarationProvider = selectedDeclarationProvider;
            _parserStatusProvider = parserStatusProvider;

            AddToCanExecuteEvaluation(SpecialEvaluateCanExecute);
        }

        private bool SpecialEvaluateCanExecute(object parameter)
        {
            if (_parserStatusProvider.Status != ParserState.Ready)
            {
                return false;
            }

            var target = FindTarget(parameter);
            return target != null && _finder.CanFind(target);
        }

        protected override void OnExecute(object parameter)
        {
            if (_parserStatusProvider.Status != ParserState.Ready)
            {
                return;
            }

            var declaration = FindTarget(parameter);
            if (declaration == null)
            {
                return;
            }

            _finder.FindAllImplementations(declaration);
        }

        protected virtual Declaration FindTarget(object parameter)
        {
            if (parameter is Declaration declaration)
            {
                return declaration;
            }

            var selectedDeclaration = _selectedDeclarationProvider.SelectedDeclaration();

            return selectedDeclaration;
        }
    }
}

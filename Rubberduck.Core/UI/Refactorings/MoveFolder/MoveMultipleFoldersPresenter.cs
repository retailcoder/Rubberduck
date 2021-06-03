﻿using Rubberduck.Refactorings.MoveFolder;
using Rubberduck.Refactorings;

namespace Rubberduck.UI.Refactorings.MoveFolder
{
    internal class MoveMultipleFoldersPresenter : RefactoringPresenterBase<MoveMultipleFoldersModel>, IMoveMultipleFoldersPresenter
    {
        private static readonly DialogData DialogData = DialogData.Create(RefactoringsUI.MoveToFolderDialog_Caption, 164, 684);

        public MoveMultipleFoldersPresenter(MoveMultipleFoldersModel model, IRefactoringDialogFactory dialogFactory) :
            base(DialogData, model, dialogFactory)
        {}
    }
}


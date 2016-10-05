﻿using System;

namespace Rubberduck.VBEditor.DisposableWrappers.VBA
{
    public class ReferenceEventArgs : EventArgs
    {
        private readonly Reference _reference;

        public ReferenceEventArgs(Reference reference)
        {
            _reference = reference;
        }

        public Reference Reference { get { return _reference; } }
    }
}
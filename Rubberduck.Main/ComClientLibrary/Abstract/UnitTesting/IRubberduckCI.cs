using Rubberduck.Resources.Registration;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rubberduck.UnitTesting
{
    [
        ComVisible(true),
        Guid(RubberduckGuid.RubberduckCIInterfaceGuid),
        InterfaceType(ComInterfaceType.InterfaceIsDual),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public interface IRubberduckCI
    {
        [Description(
            "Runs all tests in the current project and returns the results as a string. " +
            "This method is intended for use in Continuous Integration (CI) environments.")]
        [DispId(1)]
        void RunAllTests();
    }
}

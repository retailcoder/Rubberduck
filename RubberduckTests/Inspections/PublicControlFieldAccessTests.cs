using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.VBEditor.SafeComWrappers;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using RubberduckTests.Mocks;

namespace RubberduckTests.Inspections
{
    [TestFixture]
    public class PublicControlFieldAccessTests
    {
        private IVBE CreateTestProject(string clientCode, string formCode, string controlName)
        {
            return (IVBE)new MockVbeBuilder()
                .ProjectBuilder("PublicControlAccessTest", ProjectProtection.Unprotected)
                .AddComponent("ClientCode", ComponentType.StandardModule, clientCode)
                .MockUserFormBuilder("UserForm1", formCode)
                .AddControl(controlName)
                .AddFormToProjectBuilder()
                .Build().Object;
        }

        private IEnumerable<IInspectionResult> Inspect(string clientCode, string formCode, string controlName)
        {
            var vbe = CreateTestProject(clientCode, formCode, controlName);
            using (var state = MockParser.CreateAndParse(vbe))
            {
                var inspection = new PublicControlFieldAccessInspection(state);
                return inspection.GetInspectionResults(CancellationToken.None);
            }
        }

        [Test]
        [Category("Inspections")]
        public void PublicControlAccess_UsedOutsideOfForm_ReturnsResult()
        {
            const string controlName = "TextBox1";
            const string clientCode = @"
Sub Test()
    With New UserForm1
        ." + controlName + @".Text = ""Busted!""
    End With
End Sub
";
            const string formCode = @"";
            var results = Inspect(clientCode, formCode, controlName);
            Assert.AreEqual(1, results.Count());
        }

        [Test]
        [Category("Inspections")]
        public void PublicControlAccess_UsedOnlyInForm_NoResults()
        {
            const string controlName = "TextBox1";
            const string clientCode = @"
Sub Test()
    With New UserForm1
        .Show
    End With
End Sub
";
            const string formCode = @"
Private Sub " + controlName + @"_Change()
    MsgBox " + controlName + @".Text
End Sub
";
            var results = Inspect(clientCode, formCode, controlName);
            Assert.AreEqual(1, results.Count());
        }
    }
}

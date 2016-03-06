using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace privatedeveloperinc.Edizen
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("Text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class TextViewCreationListener : IWpfTextViewCreationListener
    {
        #region IWpfTextViewCreationListener Members

        public void TextViewCreated(IWpfTextView textView)
        {
            Debug.WriteLine("TextViewCreated");
            var keeper = new KeepCentered(textView);
        }

        #endregion
    }
}

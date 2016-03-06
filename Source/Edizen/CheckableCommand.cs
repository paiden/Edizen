using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace privatedeveloperinc.Edizen
{
    /// <summary>
    /// This class implements a very specific type of command: this command will count the
    /// number of times the user has clicked on it and will change its text to show this count.
    /// </summary>
    internal class CheckableCommand : OleMenuCommand
    {
        /// <summary>
        /// If a command is defined with the TEXTCHANGES flag in the VSCT file and this package is
        /// loaded, then Visual Studio will call this property to get the text to display.
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Creates a new DynamicTextCommand object with a specific CommandID and base text.
        /// </summary>
        public CheckableCommand(CommandID id, string text) :
            base(new EventHandler(ClickCallback), id, text)
        {
            this.Checked = EdizenPackage.Config.AutoSyncEnabled;
        }

        // Counter of the clicks.

        /// <summary>
        /// This is the function that is called when the user clicks on the menu command.
        /// It will check that the selected object is actually an instance of this class and
        /// increment its click counter.
        /// </summary>
        private static void ClickCallback(object sender, EventArgs args)
        {
            CheckableCommand co = (CheckableCommand)sender;
            co.Checked = !co.Checked;
            EdizenPackage.Config.AutoSyncEnabled = co.Checked;
            EdizenPackage.Config.Write();
        }
    }
}

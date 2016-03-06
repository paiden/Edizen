using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace privatedeveloperinc.Edizen
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    internal sealed class EdizenOptionsPage : DialogPage
    {
        private const string CategoryGeneral = "General";

        private double splitFraction = EdizenPackage.Config.SplitFraction;
        [Category(CategoryGeneral)]
        [DisplayName("Split Fraction")]
        [Description("Fraction from the top where the cursor should be kept. " +
            "E.g. if set to 0.5 will stay at the center of the view port, 0.0 will keep it at the top.")]
        public double SplitFraction
        {
            get { return this.splitFraction; }
            set
            {
                if (this.bottomSplitFraction >= 0.0 && (value < 0.0 || value > this.BottomSplitFraction))
                {
                    throw new ArgumentOutOfRangeException("SplitFraction", value,
                        string.Format(CultureInfo.InvariantCulture, "Value needs to be in interval [0.0, {0}=Bottom Split Fraction]",
                        this.BottomSplitFraction));
                }

                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("SplitFraction", value, "Value needs to be in interval [0.0, 1.0].");
                }

                this.splitFraction = value;
            }
        }

        private double bottomSplitFraction = EdizenPackage.Config.BottomSplitFraction;
        [Category(CategoryGeneral)]
        [DisplayName("Bottom Split Fraction")]
        [Description("Set this to a value less than 0.0 or the same as 'Split Fraction', " +
            "to keep the cursor exactly on the 'Split fraction' position. " +
            "If set to a value in the interval (Split Fraction, 1.0] the cursor will be kept within the segment defined by " +
            "the two fractions.")]
        public double BottomSplitFraction
        {
            get { return this.bottomSplitFraction; }
            set
            {
                if ((value >= 0.0 && value < this.SplitFraction) || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("BottomSplitFraction", value,
                        string.Format(CultureInfo.InvariantCulture, "Value needs to be in interval [{0}=Split Fraction, 1.0].",
                        this.SplitFraction));
                }

                this.bottomSplitFraction = value < 0.0 ? -1.0 : value;
            }
        }

        [Category(CategoryGeneral)]
        [DisplayName("Disable AutoSync while mouse pressed")]
        [Description("Disables the AutoSync operation temporarily, while the mouse pressed. " +
            "This will stop the editor from moving around when clicking or selecting text with the mouse.")]
        public bool DisableAutsyncDuringClick
        {
            get;
            set;
        }

        [Category(CategoryGeneral)]
        [DisplayName("Show error message box")]
        [Description("Showing the error message box can help diagnosing Edizen issues. This setting only applies to errors " +
            "that happen while cursor is aligned. Errors during package init or config load ... will always be shown.")]
        public bool ShowErrorMessageBox
        {
            get;
            set;
        }

        public override void LoadSettingsFromStorage()
        {
            this.Load();
            base.LoadSettingsFromStorage();
        }

        protected override void OnApply(DialogPage.PageApplyEventArgs e)
        {
            this.Apply();
        }

        private void Load()
        {
            EdizenPackage.Config.Read();

            this.splitFraction = EdizenPackage.Config.SplitFraction;
            this.DisableAutsyncDuringClick = EdizenPackage.Config.DisableAutosyncDuringClick;
            this.ShowErrorMessageBox = EdizenPackage.Config.ShowErrorMessageBox;
        }

        private void Apply()
        {
            EdizenPackage.Config.SplitFraction = this.splitFraction;
            EdizenPackage.Config.BottomSplitFraction = this.bottomSplitFraction;
            EdizenPackage.Config.DisableAutosyncDuringClick = this.DisableAutsyncDuringClick;
            EdizenPackage.Config.ShowErrorMessageBox = this.ShowErrorMessageBox;

            EdizenPackage.Config.Write();
        }
    }
}

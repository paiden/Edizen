using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace privatedeveloperinc.Edizen
{
    internal sealed class EdizenConfig
    {
        private const string SplitFractionKey = "SplitFraction";
        private const string BottomSplitFractionKey = "BottomSplitFraction";
        private const string AutosyncEnabledKey = "AutoSync";
        private const string DisableDuringClickKey = "DisableClick";
        private const string MouseDisableDelayKey = "MouseDisableDelay";
        private const string ShowErrorMsgBoxKey = "ShowErrorMessageBox";

        private readonly string path;

        private Dictionary<string, object> settings = new Dictionary<string, object>();

        public event EventHandler ConfigChanged = delegate { };

        public EdizenConfig(string configPath)
        {
            this.path = configPath;
            this.settings[SplitFractionKey] = 0.5;
            this.settings[BottomSplitFractionKey] = -1.0;
            this.settings[MouseDisableDelayKey] = 200.0;
            this.settings[AutosyncEnabledKey] = true;
            this.settings[DisableDuringClickKey] = true;
            this.settings[ShowErrorMsgBoxKey] = false;
        }

        public double SplitFraction
        {
            get
            {
                return (double)this.settings[SplitFractionKey];
            }
            set
            {
                this.settings[SplitFractionKey] = value;
                this.ConfigChanged(this, EventArgs.Empty);
            }
        }

        public double BottomSplitFraction
        {
            get
            {
                return (double)this.settings[BottomSplitFractionKey];
            }
            set
            {
                this.settings[BottomSplitFractionKey] = value;
                this.ConfigChanged(this, EventArgs.Empty);
            }
        }

        public double MouseDisableDelay
        {
            get
            {
                return (double)this.settings[MouseDisableDelayKey];
            }
            set
            {
                this.settings[MouseDisableDelayKey] = value;
                this.ConfigChanged(this, EventArgs.Empty);
            }
        }

        public bool AutoSyncEnabled
        {
            get
            {
                return (bool)this.settings[AutosyncEnabledKey];
            }
            set
            {
                this.settings[AutosyncEnabledKey] = value;
                this.ConfigChanged(this, EventArgs.Empty);
            }
        }

        public bool DisableAutosyncDuringClick
        {
            get
            {
                return (bool)this.settings[DisableDuringClickKey];
            }
            set
            {
                this.settings[DisableDuringClickKey] = value;
                this.ConfigChanged(this, EventArgs.Empty);
            }
        }

        public bool ShowErrorMessageBox
        {
            get
            {
                return (bool)this.settings[ShowErrorMsgBoxKey];
            }
            set
            {
                this.settings[ShowErrorMsgBoxKey] = value;
                this.ConfigChanged(this, EventArgs.Empty);
            }
        }

        public void Write()
        {
            try
            {
                var dir = Path.GetDirectoryName(this.path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var writer = new StreamWriter(path, append: false))
                {
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", SplitFractionKey, this.SplitFraction));
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", BottomSplitFractionKey, this.BottomSplitFraction));
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", AutosyncEnabledKey, this.AutoSyncEnabled));
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", DisableDuringClickKey, this.DisableAutosyncDuringClick));
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", MouseDisableDelayKey, this.MouseDisableDelay));
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", ShowErrorMsgBoxKey, this.ShowErrorMessageBox));
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(Application.Current.MainWindow, exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Read()
        {
            try
            {
                if (File.Exists(this.path))
                {
                    using (var reader = new StreamReader(this.path))
                    {
                        string l;
                        while ((l = reader.ReadLine()) != null)
                        {
                            var parts = l.Split(new char[] { '=' });
                            if (parts.Length == 2)
                            {
                                var key = parts[0].Trim();
                                var value = parts[1].Trim();

                                switch (key)
                                {
                                    case SplitFractionKey: SplitFraction = double.Parse(value, CultureInfo.InvariantCulture); break;
                                    case AutosyncEnabledKey: AutoSyncEnabled = bool.Parse(value); break;
                                    case DisableDuringClickKey: DisableAutosyncDuringClick = bool.Parse(value); break;
                                    case BottomSplitFractionKey: BottomSplitFraction = double.Parse(value, CultureInfo.InvariantCulture); break;
                                    case MouseDisableDelayKey: MouseDisableDelay = double.Parse(value, CultureInfo.InvariantCulture); break;
                                    case ShowErrorMsgBoxKey: ShowErrorMessageBox = bool.Parse(value); break;
                                }
                            }
                        }
                    }
                }

                this.SplitFraction = Clamp(this.SplitFraction, 0.0, 1.0);
                this.BottomSplitFraction = this.BottomSplitFraction < 0.0 ? -1.0 : Clamp(this.BottomSplitFraction, this.SplitFraction, 1.0);

            }
            catch (Exception exc)
            {
                MessageBox.Show(Application.Current.MainWindow, exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (Comparer<T>.Default.Compare(value, min) < 0) { return min; }
            if (Comparer<T>.Default.Compare(value, max) > 0) { return max; }

            return value;
        }
    }
}

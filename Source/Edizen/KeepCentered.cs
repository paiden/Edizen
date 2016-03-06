using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace privatedeveloperinc.Edizen
{
    internal sealed class KeepCentered
    {
        private static readonly List<KeepCentered> keepers = new List<KeepCentered>();
        private readonly IWpfTextView view;

        public KeepCentered(IWpfTextView view)
        {
            this.view = view;

            this.view.Caret.PositionChanged += HandleCaretPositionChanged;
            this.view.Closed += HandleViewClosed;
            KeepCentered.keepers.Add(this);
        }

        public static void TriggerAlignOnAllKeepers()
        {
            for (int i = 0; i < keepers.Count; i++)
            {
                keepers[i].TriggerAlignCaret();
            }
        }

        public void TriggerAlignCaret()
        {
            var newLine = this.view.GetTextViewLineContainingBufferPosition(this.view.Caret.Position.BufferPosition);
            this.AlignCaret(newLine);
        }

        private void HandleViewClosed(object sender, EventArgs _)
        {
            this.view.Caret.PositionChanged -= HandleCaretPositionChanged;
            this.view.Closed -= HandleViewClosed;
            KeepCentered.keepers.Remove(this);
        }

        private void HandleCaretPositionChanged(object sender, CaretPositionChangedEventArgs _)
        {
            this.Align();
        }

        private void Align()
        {
            try
            {
                if (ShouldAutosync())
                {
                    var newLine = this.view.GetTextViewLineContainingBufferPosition(this.view.Caret.Position.BufferPosition);
                    this.AlignCaret(newLine);
                }
            }
            catch (Exception exc)
            {
                if (EdizenPackage.Config.ShowErrorMessageBox)
                {
                    MessageBox.Show(Application.Current.MainWindow, exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AlignCaret(IWpfTextViewLine newLine)
        {
            if (EdizenPackage.Config.BottomSplitFraction < 0.0)
            {
                this.AlignExact();
            }
            else
            {
                this.AlignInSegment(newLine);
            }
        }

        private void AlignExact()
        {
            var offset = this.view.ViewportHeight * EdizenPackage.Config.SplitFraction;
            this.view.DisplayTextLineContainingBufferPosition(this.view.Caret.Position.BufferPosition, offset, ViewRelativePosition.Top);
        }

        private void AlignInSegment(IWpfTextViewLine newLine)
        {
            var lineTopInView = newLine.TextTop - newLine.VisibleArea.Top;
            var lineBottomInView = newLine.TextBottom - newLine.VisibleArea.Top;

            var segmentTop = this.view.ViewportHeight * EdizenPackage.Config.SplitFraction;
            var segmentBottom = this.view.ViewportHeight * EdizenPackage.Config.BottomSplitFraction - newLine.TextHeight;

            var pos = this.view.Caret.Position.BufferPosition;
            if (lineTopInView < segmentTop)
            {
                this.view.DisplayTextLineContainingBufferPosition(pos, segmentTop, ViewRelativePosition.Top);
            }
            else if (lineBottomInView > segmentBottom)
            {
                this.view.DisplayTextLineContainingBufferPosition(pos, segmentBottom, ViewRelativePosition.Top);
            }
        }

        private static bool ShouldAutosync()
        {
            bool autoSyncEnabled = EdizenPackage.Config.AutoSyncEnabled;
            bool mouseButtonPressed = Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed;
            mouseButtonPressed |= IsMousePressedDelayFix();
            bool disableClick = EdizenPackage.Config.DisableAutosyncDuringClick && mouseButtonPressed;
            return autoSyncEnabled && !disableClick;
        }

        /// <summary>
        /// When text is selected in the text editor, the caret changed event is fired after the mouse up event -> mouse is not classified 
        /// as pressed anymore. So we hook the global preview mouse events and log the last up event, so we can disable the 
        /// mouse for a small time duration event after the up phase.
        /// </summary>
        /// <returns></returns>
        private static bool IsMousePressedDelayFix()
        {
            return (DateTime.UtcNow - EdizenPackage.LastMouseUp).TotalMilliseconds < EdizenPackage.Config.MouseDisableDelay;
        }
    }
}

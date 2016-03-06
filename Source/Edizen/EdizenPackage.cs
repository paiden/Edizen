﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace privatedeveloperinc.Edizen
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(GuidList.guidEdizenPkgString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.CodeWindow)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideOptionPage(typeof(EdizenOptionsPage), "Edizen", "General", 0, 0, true)]
    public sealed class EdizenPackage : Package
    {
        private const string PackageName = "Edizen";
        internal static DateTime LastMouseUp { get; private set; }

        internal static EdizenConfig Config { get; private set; }
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public EdizenPackage()
        {
            try
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PackageName);
                path = Path.Combine(path, "config.ini");
                Config = new EdizenConfig(path);
                Config.Read();
                Config.ConfigChanged += HandleConfigChanged;

                Application.Current.MainWindow.PreviewMouseUp += PreviewMouseUp;
            }
            catch (Exception exc)
            {
                MessageBox.Show(Application.Current.MainWindow, exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                LastMouseUp = DateTime.UtcNow;
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            try
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
                base.Initialize();

                // Add our command handlers for menu (commands must exist in the .vsct file)
                OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
                if (null != mcs)
                {
                    // Create the command for the menu item.
                    const int cmdIdEnableEdizen = 0x100;
                    CommandID menuCommandID = new CommandID(GuidList.guidEdizenCmdSet, cmdIdEnableEdizen);
                    var menuItem = new CheckableCommand(menuCommandID, "Test");
                    mcs.AddCommand(menuItem);

                    const int cmdIdSyncEdizen = 0x101;
                    CommandID syncComamndID = new CommandID(GuidList.guidEdizenCmdSet, cmdIdSyncEdizen);
                    var menuItemSyc = new MenuCommand(HandleSyncNowCommand, syncComamndID);
                    mcs.AddCommand(menuItemSyc);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(Application.Current.MainWindow, exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void HandleSyncNowCommand(object sender, EventArgs args)
        {
            KeepCentered.TriggerAlignOnAllKeepers();
        }

        private void HandleConfigChanged(object sender, EventArgs args)
        {
            if (Config.AutoSyncEnabled)
            {
                KeepCentered.TriggerAlignOnAllKeepers();
            }
        }
    }
}
using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;

namespace CloseAndDelete
{
    internal sealed class CloseAndDeleteCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("acff2309-824d-4bb9-b3d9-766f164d13e0");

        readonly Package _package;

        CloseAndDeleteCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public static CloseAndDeleteCommand Instance { get; private set; }

        IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new CloseAndDeleteCommand(package);
        }

        void MenuItemCallback(object sender, EventArgs e)
        {
            if (ShowMessage("Are you sure?", OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_YESNO) != (int)VSConstants.MessageBoxResult.IDYES)
            {
                return;
            }

            if (ServiceProvider.GetService(typeof(IVsSolution)) is IVsSolution solution)
            {
                try
                {
                    if (solution.GetSolutionInfo(out var solutionDirectory, out _, out _) == VSConstants.S_OK && !string.IsNullOrEmpty(solutionDirectory))
                    {
                        if (solution.CloseSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave, null, 0) == VSConstants.S_OK)
                        {
                            DeleteDirectoryRetry(solutionDirectory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.ToString(), OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK);
                }
            }
        }

        int ShowMessage(string message, OLEMSGICON icon, OLEMSGBUTTON buttons)
        {
            return VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                message,
                string.Empty,
                icon,
                buttons,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        void DeleteDirectoryRetry(string directory)
        {
            while (true)
            {
                try
                {
                    Directory.Delete(directory, true);
                    break;
                }
                catch (IOException ex)
                {
                    if (ShowMessage(ex.Message, OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL) == (int)VSConstants.MessageBoxResult.IDRETRY)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}

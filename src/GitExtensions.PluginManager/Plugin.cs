using GitUIPluginInterfaces;
using ResourceManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.PluginManager
{
    /// <summary>
    /// GitExtensions plugin for backuping using bundles.
    /// </summary>
    public class Plugin : GitPluginBase
    {
        public const string GitExtensionsRelativePath = @"GitExtensions.exe";
        public const string PluginManagerRelativePath = @"PluginManager\PackageManager.UI.exe";
        public const string PluginsPackageNameFormat = "--dependencies GitExtensions.Plugins-v{0}.{1}";
        public static readonly List<string> FrameworkMonikers = new List<string>() { "net461", "net462", "any", "netstandard2.0" };

        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Plugin Manager";
            Description = "Plugin Manager";
        }

        public override void Register(IGitUICommands commands)
        {
            base.Register(commands);
            Configuration = new PluginSettings(Settings);
        }

        public override IEnumerable<ISetting> GetSettings()
            => Configuration;

        public override bool Execute(GitUIBaseEventArgs gitUiCommands)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string pluginsPath = Path.Combine(basePath, "Plugins");

            FileVersionInfo version = FileVersionInfo.GetVersionInfo(Path.Combine(basePath, GitExtensionsRelativePath));

            List<string> args = new List<string>();
            args.Add($"--path \"{pluginsPath}\"");
            args.Add(String.Format(PluginsPackageNameFormat, version.ProductMajorPart, version.ProductMinorPart));
            args.Add($"--monikers {String.Join(",", FrameworkMonikers)}");

            if (Uri.IsWellFormedUriString(Configuration.PackageSourceUrl, UriKind.Absolute))
                args.Add($"--packagesource {Configuration.PackageSourceUrl}");

            ProcessStartInfo info = new ProcessStartInfo(
                Path.Combine(pluginsPath, PluginManagerRelativePath),
                String.Join(" ", args)
            );
            info.UseShellExecute = false;
            info.Verb = "runas";

            bool? isConfirmed = IsShutdownConfirmed();
            if (isConfirmed != null)
            {
                Process.Start(info);

                if (isConfirmed.Value)
                {
                    CloseAllOtherInstances();
                    Application.Exit();
                }
            }

            return true;
        }

        private bool? IsShutdownConfirmed()
        {
            if (!Configuration.AskToCloseInstances)
                return true;

            DialogResult result = MessageBox.Show(
                "Plugin Manager (based on your actions) may try to write files to your GitExtensions installation folder. " + Environment.NewLine +
                "All instances of GitExtensions should be closed to avoid locks." + Environment.NewLine +
                "Do you want to kill all instances of GitExtensions?",
                "GitExtensions - PluginManager",
                MessageBoxButtons.YesNoCancel
            );

            if (result == DialogResult.Yes)
                return true;
            else if (result == DialogResult.No)
                return false;

            return null;
        }

        private void CloseAllOtherInstances()
        {
            Process current = Process.GetCurrentProcess();
            foreach (Process other in Process.GetProcesses())
            {
                try
                {
                    if (other.MainModule.FileName == current.MainModule.FileName && other.Id != current.Id)
                        other.Kill();
                }
                catch (Win32Exception)
                {
                    continue;
                }
            }
        }
    }
}

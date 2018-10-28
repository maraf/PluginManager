using GitExtensions.PluginManager.Properties;
using GitUIPluginInterfaces;
using ResourceManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
    [Export(typeof(IGitPlugin))]
    public class Plugin : GitPluginBase
    {
        public const string PackageId = @"GitExtensions.PluginManager";
        public const string GitExtensionsRelativePath = @"GitExtensions.exe";
        public const string PluginManagerRelativePath = @"PackageManager\PackageManager.UI.exe";
        public static readonly List<string> FrameworkMonikers = new List<string>() { "net461", "net462", "any", "netstandard2.0" };

        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Plugin Manager";
            Description = "Plugin Manager";
            Icon = Resources.Icon;
        }

        public override void Register(IGitUICommands commands)
        {
            base.Register(commands);
            Configuration = new PluginSettings(Settings);
        }

        public override IEnumerable<ISetting> GetSettings()
            => Configuration;

        public override bool Execute(GitUIEventArgs gitUiCommands)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string pluginsPath = Path.Combine(basePath, "Plugins");

            List<string> args = new List<string>();
            args.Add($"--path \"{pluginsPath}\"");
            args.Add($"--dependencies GitExtensions.Plugins");
            args.Add($"--monikers {String.Join(",", FrameworkMonikers)}");
            args.Add($"--selfpackageid {PackageId}");
            args.Add($"--processnamestokillbeforechange \"{Process.GetCurrentProcess().ProcessName}\"");

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = Path.Combine(pluginsPath, PluginManagerRelativePath),
                Arguments = String.Join(" ", args),
                UseShellExecute = false,
                Verb = "runas"
            };
            Process.Start(info);

            if (Configuration.CloseInstances)
            {
                CloseAllOtherInstances();
                Application.Exit();
            }

            return true;
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

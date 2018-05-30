using GitUIPluginInterfaces;
using ResourceManager;
using System;
using System.Collections.Generic;
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
    public class Plugin : GitPluginBase, IGitPluginForRepository
    {
        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Plugin Manager";
            Description = "Plugin Manager";
        }

        public override bool Execute(GitUIBaseEventArgs gitUiCommands)
        {
            string basePath = @"C:\Program Files (x86)\GitExtensions";
            string pluginsPath = Path.Combine(basePath, "Plugins");

            FileVersionInfo version = FileVersionInfo.GetVersionInfo(Path.Combine(basePath, "GitExtensions.exe"));

            ProcessStartInfo info = new ProcessStartInfo(
                Path.Combine(pluginsPath, @"PluginManager\PackageManager.UI.exe"),
                $"--path \"{pluginsPath}\" " +
                $"--dependencies GitExtensions.Plugins-v{version.ProductMajorPart}.{version.ProductMinorPart} " +
                $"--monikers net461,net462,any,netstandard2.0 " +
                $"--packagesource {Configuration.PackageSourceUrl}"
            );
            info.UseShellExecute = false;
            info.Verb = "runas";

            Process.Start(info);
            return true;
        }

        public override IEnumerable<ISetting> GetSettings()
            => Configuration;

        public override void Register(IGitUICommands commands)
        {
            base.Register(commands);

            Configuration = new PluginSettings(Settings);
        }
    }
}

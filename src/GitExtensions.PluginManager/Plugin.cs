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

            Process.Start(info);
            return true;
        }
    }
}

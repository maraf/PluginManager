using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Neptuo;
using PackageManager.Models;

namespace PackageManager.Services
{
    internal class SelfUpdateService : ISelfUpdateService
    {
        private readonly App application;

        public SelfUpdateService(App application)
        {
            Ensure.NotNull(application, "application");
            this.application = application;
        }

        public bool IsSelfUpdate => application.Args.IsSelfUpdate;

        public void Update(IPackage latest)
        {
            // Copy to temp.
            string current = Assembly.GetExecutingAssembly().Location;
            string temp = Path.Combine(Path.GetTempPath(), "PMUI" + Guid.NewGuid().ToString().Replace("-", string.Empty) + ".exe");
            File.Copy(current, temp, true);

            // Rerun with self update.
            application.Args.IsSelfUpdate = true;
            string arguments = application.Args.ToString();

            ProcessStartInfo processStart = new ProcessStartInfo(
                temp,
                arguments
            );

            processStart.Verb = "runas";
            Process.Start(processStart);

            application.Shutdown();
        }

        public void RunNewInstance(IPackage package)
        {
            string target = Directory
                .EnumerateFiles(application.Args.Path, "PackageManager.UI.exe", SearchOption.AllDirectories)
                .FirstOrDefault();

            if (target != null)
            {
                application.Args.IsSelfUpdate = false;
                string arguments = application.Args.ToString();

                ProcessStartInfo processStart = new ProcessStartInfo(
                    target,
                    arguments
                );

                processStart.Verb = "runas";
                Process.Start(processStart);
            }
        }
    }
}

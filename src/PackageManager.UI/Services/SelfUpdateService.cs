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
    internal partial class SelfUpdateService : ISelfUpdateService
    {
        private readonly IApplication application;
        private readonly ProcessService processes;

        public SelfUpdateService(IApplication application, ProcessService processes)
        {
            Ensure.NotNull(application, "application");
            Ensure.NotNull(processes, "processes");
            this.application = application;
            this.processes = processes;
        }

        public string CurrentFileName => Path.GetFileName(Assembly.GetExecutingAssembly().Location);

        public bool IsSelfUpdate => application.Args.IsSelfUpdate;

        public void Update(IPackage latest)
        {
            string current = Assembly.GetExecutingAssembly().Location;
            string temp = CopySelfToTemp(current);
            IArgs arguments = CreateArguments(current, latest);
            RerunFromTemp(temp, arguments);
        }

        private string CopySelfToTemp(string current)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", string.Empty));
            string temp = Path.Combine(tempDirectory, CurrentFileName);
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            File.Copy(current, temp, true);
            return temp;
        }

        private void RerunFromTemp(string temp, IArgs arguments)
        {
            processes.Run(temp, arguments.ToString());
            application.Shutdown();
        }

        private IArgs CreateArguments(string current, IPackageIdentity package)
        {
            IArgs args = application.Args.Clone();

            args.IsSelfUpdate = true;
            args.SelfOriginalPath = current;
            args.SelfUpdateVersion = package.Version;

            return args;
        }

        public void RunNewInstance(IPackage package)
        {
            string target = null;
            if (application.Args.SelfOriginalPath != null)
            {
                target = application.Args.SelfOriginalPath;
            }
            else
            {
                target = Directory
                    .EnumerateFiles(application.Args.Path, CurrentFileName, SearchOption.AllDirectories)
                    .FirstOrDefault();
            }

            if (target != null)
            {
                IArgs args = application.Args.Clone();

                args.IsSelfUpdate = false;
                args.SelfUpdateVersion = null;

                string arguments = args.ToString();
                processes.Run(target, arguments);
            }
        }
    }
}

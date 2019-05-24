using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    internal partial class ProcessService
    {
        private const int ErrorCancelled = 1223;

        private readonly IApplication application;
        private readonly IReadOnlyCollection<string> toKillNames;

        public ProcessService(IApplication application, IReadOnlyCollection<string> toKillNames)
        {
            Ensure.NotNull(application, "application");
            Ensure.NotNull(toKillNames, "toKillNames");
            this.application = application;
            this.toKillNames = toKillNames;
        }

        public void RestartAsAdministrator()
        {
            Process current = Process.GetCurrentProcess();
            ProcessStartInfo processStart = new ProcessStartInfo(
                current.MainModule.FileName,
                application.Args.ToString()
            );

            processStart.Verb = "runas";

            try
            {
                Process.Start(processStart);
            }
            catch (Win32Exception e) when (e.NativeErrorCode == ErrorCancelled)
            {
                throw new RestartAsAdministratorCancelledException(e);
            }

            application.Shutdown();
        }

        public void Run(string filePath, string arguments)
        {
            ProcessStartInfo processStart = new ProcessStartInfo(
                filePath,
                arguments
            );

            Process.Start(processStart);
        }

        public ProcessKillContext PrepareContextForProcessesKillBeforeChange() 
            => new ProcessKillContext(toKillNames);
    }
}

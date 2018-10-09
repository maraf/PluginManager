using Neptuo;
using Neptuo.Logging;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLevel = Neptuo.Logging.LogLevel;
using NuGetLogLevel = NuGet.Common.LogLevel;

namespace PackageManager.Logging
{
    public class NuGetLogger : ILogger
    {
        private readonly ILog log;

        public NuGetLogger(ILog log)
        {
            Ensure.NotNull(log, "log");
            this.log = log;
        }

        private LogLevel MapLevel(NuGetLogLevel level)
        {
            switch (level)
            {
                case NuGetLogLevel.Debug:
                    return LogLevel.Debug;
                case NuGetLogLevel.Verbose:
                    return LogLevel.Debug;
                case NuGetLogLevel.Information:
                    return LogLevel.Info;
                case NuGetLogLevel.Minimal:
                    return LogLevel.Info;
                case NuGetLogLevel.Warning:
                    return LogLevel.Warning;
                case NuGetLogLevel.Error:
                    return LogLevel.Error;
                default:
                    throw Ensure.Exception.NotSupported(level);
            }
        }

        public void Log(NuGetLogLevel level, string data)
        {
            log.Log(MapLevel(level), data);
        }

        public void Log(ILogMessage message)
        {
            log.Log(MapLevel(message.Level), message.Message);
        }

        public Task LogAsync(NuGetLogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        public void LogDebug(string data) => Log(NuGetLogLevel.Debug, data);
        public void LogError(string data) => Log(NuGetLogLevel.Error, data);
        public void LogInformation(string data) => Log(NuGetLogLevel.Information, data);
        public void LogInformationSummary(string data) => Log(NuGetLogLevel.Information, data);
        public void LogMinimal(string data) => Log(NuGetLogLevel.Minimal, data);
        public void LogVerbose(string data) => Log(NuGetLogLevel.Verbose, data);
        public void LogWarning(string data) => Log(NuGetLogLevel.Warning, data);
    }
}

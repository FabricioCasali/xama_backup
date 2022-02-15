using System.IO;

using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

using XamaWinService.Configs;

namespace XamaWinService.Helpers
{
    public static class NLogInit
    {
        public static void Configure(ConfigLog log)
        {
            if (log == null)
                return;

            if (LogManager.Configuration != null)
            {
                LogManager.Configuration = null;
            }

            var config = new LoggingConfiguration();
            var console = new ColoredConsoleTarget("logconsole");
            var logFile = new FileTarget("logfile")
            {
                FileName = Path.Combine(log.LogFilePath, log.LogFileName),
                MaxArchiveFiles = log.MaxArchiveFiles,
                ArchiveAboveSize = log.MaxLogSize
            };
            var wrapper = new AsyncTargetWrapper(logFile, 5000, AsyncTargetWrapperOverflowAction.Discard);

            config.AddRule(log.ShowTrace ? NLog.LogLevel.Trace : NLog.LogLevel.Debug, NLog.LogLevel.Fatal, console);
            config.AddRule(log.ShowTrace ? NLog.LogLevel.Trace : NLog.LogLevel.Debug, NLog.LogLevel.Fatal, wrapper);
            LogManager.Configuration = config;
        }

    }
}

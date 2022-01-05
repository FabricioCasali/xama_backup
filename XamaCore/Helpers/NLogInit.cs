using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.IO;
using XamaCore.Configs;

namespace XamaCore.Helpers
{
    public static class NLogInit
    {
        public static void Configure(ConfigApp ca)
        {
            if (ca.LogConfig == null)
                return;

            if (LogManager.Configuration != null)
            {
                LogManager.Configuration = null;
            }

            var config = new LoggingConfiguration();
            var console = new ColoredConsoleTarget("logconsole");
            var logFile = new FileTarget("logfile")
            {
                FileName = Path.Combine(ca.LogConfig.LogFilePath, ca.LogConfig.LogFileName),
                MaxArchiveFiles = ca.LogConfig.MaxArchiveFiles,
                ArchiveAboveSize = ca.LogConfig.MaxLogSize
            };

            var wrapper = new AsyncTargetWrapper(logFile, 5000, AsyncTargetWrapperOverflowAction.Discard);
            config.AddRule(ca.LogConfig.ShowTrace ? NLog.LogLevel.Trace : NLog.LogLevel.Debug, NLog.LogLevel.Fatal, wrapper);
            config.AddRule(ca.LogConfig.ShowTrace ? NLog.LogLevel.Trace : NLog.LogLevel.Debug, NLog.LogLevel.Fatal, console);
            LogManager.Configuration = config;
        }

    }
}
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using XamaCore.Configs;

namespace XamaCore
{
    public static class NLogInit
    {
        public static void Configure(ConfigApp ca)
        {
            if (ca.LogConfig == null)
                return;

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

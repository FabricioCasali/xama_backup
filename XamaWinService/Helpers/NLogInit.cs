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
            var cTarget = new ColoredConsoleTarget("logconsole");
            var fTarget = new FileTarget("logfile")
            {
                FileName = Path.Combine(log.LogFilePath, log.LogFileName),
                MaxArchiveFiles = log.MaxArchiveFiles,
                ArchiveAboveSize = log.MaxLogSize
            };
            var wTarget = new AsyncTargetWrapper(fTarget, 5000, AsyncTargetWrapperOverflowAction.Discard);
            var nTarget = new NullTarget("null");

            config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, wTarget, "Quartz*", true);
            config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, cTarget, "Quartz*", true);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Debug, nTarget, "Quartz*", true);

            config.AddRule(log.ShowTrace ? NLog.LogLevel.Trace : NLog.LogLevel.Debug, NLog.LogLevel.Fatal, cTarget, "*");
            config.AddRule(log.ShowTrace ? NLog.LogLevel.Trace : NLog.LogLevel.Debug, NLog.LogLevel.Fatal, wTarget, "*");

            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
        }

    }
}

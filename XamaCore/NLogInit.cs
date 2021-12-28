using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using XamaCore.Configs;

namespace XamaCore
{
    public class NLogInit
    {
        public LogFactory Configure(ConfigApp ca)
        {
            if (ca.LogConfig == null)
                return null;

            var config = new LoggingConfiguration();
            var console = new ColoredConsoleTarget("logconsole");
            var logFile = new FileTarget("logfile")
            {
                FileName = Path.Combine(ca.LogConfig.LogFilePath, ca.LogConfig.LogFileName),
                MaxArchiveFiles = ca.LogConfig.MaxArchiveFiles,
                ArchiveAboveSize = ca.LogConfig.MaxLogSize
            };

            var wrapper = new AsyncTargetWrapper(logFile, 5000, AsyncTargetWrapperOverflowAction.Discard);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, wrapper);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, console);
            var lf = new LogFactory();
            lf.Configuration = config;
            return lf;
        }

    }
}

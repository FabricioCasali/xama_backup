using System.IO;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Topshelf;
using XamaCore.Configs;

namespace XamaCore
{
    class Program
    {

        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                // todo : add exception if no config file
                ConfigApp appConfig = new ConfigApp();

                if (File.Exists("./app.json"))
                    appConfig = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText("./app.json"));

                x.Service<CoreService>(s =>
                {
                    s.ConstructUsing(name => new CoreService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                if (appConfig.EnableLog)
                {
                    x.UseNLog(new NLogInit().Configure(appConfig));
                }
                x.SetDescription("Xamã Core Service");
                x.SetDisplayName("Xamã Core Service");
                x.SetServiceName("XamaCoreService");
            });
        }
    }

    class NLogInit
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

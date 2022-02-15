using System;
using System.IO;

using Common.Logging;

using Topshelf;
using Topshelf.Autofac;

using XamaWinService.Configs;
using XamaWinService.Helpers;

namespace XamaWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
            if (!File.Exists(cfgPath))
                throw new FileNotFoundException($"missing config file: {cfgPath}");

            var cfg = ConfigLoader.ReadConfigFile(cfgPath);

            NLogInit.Configure(cfg.LogConfig);

            var container = Bootstrapper.BuildContainer(cfg);

            var exitCode = HostFactory.Run(p =>
            {
                p.UseAutofacContainer(container);
                p.OnException(ex =>
                {
                    LogManager.GetLogger("system_core").Error(ex);
                });
                p.Service((Action<Topshelf.ServiceConfigurators.ServiceConfigurator<CoreService>>)(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted((s, h) => s.Start(h));
                    s.WhenStopped((s, h) => s.Stop(h));
                }));
                p.RunAsLocalSystem();
                p.SetDescription("Xamã Core Service");
                p.SetDisplayName("Xamã Core Service");
                p.SetServiceName("XamaCoreService");
                p.StartAutomatically();
                p.RunAsLocalSystem();

            });
        }
    }
}

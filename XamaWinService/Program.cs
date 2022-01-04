using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using NLog;

using Topshelf;
using Topshelf.Autofac;

using XamaCore.Configs;
using XamaCore.Helpers;

namespace XamaWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("./app.json"))
                throw new FileNotFoundException($"missing config file : app.json");


            var jss = new JsonSerializerSettings();
            jss.Converters.Add(new StringEnumConverter());
            var cfg = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText("./app.json"), jss);

            NLogInit.Configure(cfg);

            var container = Bootstrapper.BuildContainer(cfg);

            var exitCode = HostFactory.Run(p =>
            {
                p.UseAutofacContainer(container);
                p.OnException(ex =>
                {
                    LogManager.GetCurrentClassLogger().Error(ex);
                });
                p.Service((Action<Topshelf.ServiceConfigurators.ServiceConfigurator<CoreService>>)(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                }));
                p.RunAsLocalSystem();
                p.SetDescription("Xamã Core Service");
                p.SetDisplayName("Xamã Core Service");
                p.SetServiceName("XamaCoreService");

            });
        }
    }
}

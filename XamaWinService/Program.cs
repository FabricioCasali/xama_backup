using System;
using System.IO;
using Newtonsoft.Json;
using Topshelf;
using Topshelf.Autofac;
using XamaCore;
using XamaCore.Configs;

namespace XamaWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            // todo : add exception if no config file 
            ConfigApp appConfig = new ConfigApp();

            if (File.Exists("./app.json"))
                appConfig = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText("./app.json"));

            var container = Bootstrapper.BuildContainer(appConfig);

            var rc = HostFactory.Run(x =>
            {

                x.UseAutofacContainer(container);
                x.Service<CoreService>(s =>
                {
                    s.ConstructUsingAutofacContainer();
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
}

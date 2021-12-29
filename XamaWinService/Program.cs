using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            if (!File.Exists("./app.json"))
                throw new FileNotFoundException($"missing config file : app.json");

            var jss = new JsonSerializerSettings();
            jss.Converters.Add(new StringEnumConverter());
            var cfg = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText("./app.json"), jss);

            var container = Bootstrapper.BuildContainer(cfg);

            var exitCode = HostFactory.Run(p =>
            {

                p.UseAutofacContainer(container);
                p.Service<CoreService>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                p.RunAsLocalSystem();
                if (cfg.EnableLog)
                {
                    p.UseNLog(new NLogInit().Configure(cfg));
                }
                p.SetDescription("Xamã Core Service");
                p.SetDisplayName("Xamã Core Service");
                p.SetServiceName("XamaCoreService");

            });
        }
    }
}

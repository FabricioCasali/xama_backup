using Autofac;
using Autofac.Extras.Quartz;
using LiteDB;
using System.Collections.Specialized;
using XamaCore;
using XamaCore.Compressors;
using XamaCore.Configs;

namespace XamaWinService
{
    static class Bootstrapper
    {
        public static IContainer BuildContainer(ConfigApp cfg)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CoreService>();

            var schedulerConfig = new NameValueCollection
        {
            { "quartz.scheduler.instanceName", "CoreScheduler" },
            { "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
            { "quartz.threadPool.threadCount", "3" }
        };

            builder.RegisterModule(new QuartzAutofacFactoryModule
            {
                ConfigurationProvider = c => schedulerConfig
            });


            var db = LiteDBINit.Init(cfg);
            var lr = new LiteRepository(db);
            builder.RegisterInstance<LiteDatabase>(db).SingleInstance();
            builder.RegisterInstance<LiteRepository>(lr).SingleInstance();
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(BackupJob).Assembly));
            builder.Register<ConfigApp>(c => cfg).SingleInstance();
            builder.RegisterType<BackupProcessor>();

            builder.RegisterType<CompressZip>().Named<ICompress>("zip");
            builder.RegisterType<Compress7zip>().Named<ICompress>("7zip");

            var container = builder.Build();
            return container;
        }
    }
}

using XamaCore.Configs;
using Autofac;
using System.Collections.Specialized;
using Autofac.Extras.Quartz;

namespace XamaCore
{
    static class Bootstrapper
    {
     public static IContainer BuildContainer(ConfigApp appConfig)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<CoreService>();

        var schedulerConfig = new NameValueCollection
        {
            { "quartz.scheduler.instanceName", "MyScheduler" },
            { "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
            { "quartz.threadPool.threadCount", "3" }
        };

        builder.RegisterModule(new QuartzAutofacFactoryModule
        {
            ConfigurationProvider = c => schedulerConfig
        });

        builder.RegisterModule(new QuartzAutofacJobsModule(typeof(BackupJob).Assembly));
        builder.Register<ConfigApp>(c => appConfig).SingleInstance();

        var container = builder.Build();
        return container;
    }
    }
}

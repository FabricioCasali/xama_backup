using System.Collections.Specialized;
using System.Linq;

using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;
using Autofac.Extras.Quartz;

using LiteDB;

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
            builder.RegisterType<CompressZip>().Named<ICompress>(ConfigCompressionMethod.Zip.ToString());
            builder.RegisterType<Compress7zip>().Named<ICompress>(ConfigCompressionMethod.SevenZip.ToString());
            builder.RegisterType<BackupProcessor>().WithParameter(new ResolvedParameter(
                (pi, ctx) => pi.ParameterType == typeof(ICompress),
                (pi, ctx) =>
                {
                    var list = ((ResolveRequestContext)ctx).Parameters.ToList();
                    var p = list.FirstOrDefault() as NamedParameter;
                    return ctx.ResolveNamed<ICompress>(p.Value.ToString());
                }));

            var container = builder.Build();
            return container;
        }
    }
}

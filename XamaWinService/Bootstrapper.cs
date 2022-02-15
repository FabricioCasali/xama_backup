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

using XamaWinService.Configs;
using XamaWinService.Helpers;

namespace XamaWinService
{
    public static class Bootstrapper
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
            builder.RegisterInstance<LiteDatabase>(db).SingleInstance().AsSelf().As<ILiteDatabase>();
            builder.RegisterInstance<LiteRepository>(lr).SingleInstance().AsSelf().As<ILiteRepository>();
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

            var types = TypeHelper.GetTypesExtending<Schedulers.IScheduler>(true, true);
            foreach (var t in types)
            {
                builder.RegisterType(t).Named(t.BaseType.GenericTypeArguments[0].Name, typeof(Schedulers.IScheduler));
            }

            builder.RegisterType<ScheduleConverter>().AsSelf();
            var container = builder.Build();

            return container;
        }
    }
}

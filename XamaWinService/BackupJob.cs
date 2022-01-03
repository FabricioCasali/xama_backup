using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using LiteDB;
using NLog;
using Quartz;
using System.Threading.Tasks;
using XamaCore;
using XamaCore.Configs;

namespace XamaWinService
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private ILifetimeScope _scope;
        private LiteRepository _rep;

        public BackupJob(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;
            var processor = _scope.Resolve<BackupProcessor>(new NamedParameter("compress", task.Target.CompressionMethod.ToString()));
            var result = processor.Process(task);

            return Task.CompletedTask;
        }
    }
}

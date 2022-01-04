using System.Threading.Tasks;

using Autofac;

using NLog;

using Quartz;

using XamaCore;
using XamaCore.Configs;

namespace XamaWinService
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private ILifetimeScope _scope;

        public BackupJob(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;
            var processor = _scope.Resolve<BackupProcessor>(new NamedParameter("compress", task.Target.CompressionMethod.ToString()));
            var result = processor.ProcessTask(task);

            return Task.CompletedTask;
        }
    }
}

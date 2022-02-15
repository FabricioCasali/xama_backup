using System.Threading.Tasks;

using Autofac;

using LiteDB;

using NLog;

using Quartz;

using XamaCore;
using XamaCore.Configs;
using XamaCore.Data;
using XamaCore.Events;

namespace XamaWinService
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private ILifetimeScope _scope;
        private ILiteRepository _repository;

        public BackupJob(ILifetimeScope scope, ILiteRepository repository)
        {
            _scope = scope;
            _repository = repository;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;
            var processor = _scope.Resolve<BackupProcessor>(new NamedParameter("compress", task.Target.CompressionMethod.ToString()));
            processor.FileCopied += HandleFileCopied;
            var result = processor.ProcessTask(task);
            _repository.Insert<BackupInfo>(result, "backup_info");
            return Task.CompletedTask;
        }

        private void HandleFileCopied(object sender, FileCopiedEventArgs e)
        {
            _repository.Insert<BackupFile>(e.File, "backup_file");
        }
    }
}

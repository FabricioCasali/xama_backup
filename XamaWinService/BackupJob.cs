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

        private ILiteDatabase _db;

        public BackupJob(ILifetimeScope scope, ILiteRepository repository, ILiteDatabase db)
        {
            _scope = scope;
            _repository = repository;
            _db = db;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;
            var processor = _scope.Resolve<BackupProcessor>(new NamedParameter("compress", task.Target.CompressionMethod.ToString()));
            processor.FileCopied += HandleFileCopied;

            BackupInfo result;
            BackupInfo comparison = null;
            BackupType nextType = BackupType.Complete;

            if (task.Target.Retention != null)
            {
                var fn = task.Target.FileName;
                var fr = new BackupFileHelper(task.Target.Path, task.Target.FileName, task.Target.Retention.NumberOfFullCopies, task.Target.Retention.FullCopieEvery);
                fr.CheckPath();
                fr.ClearPath();
                nextType = fr.NextBackupShouldBe();
                BackupArchiveDetail last = null;
                if (nextType != BackupType.Complete)
                {
                    if (task.TaskType == ConfigTaskTypeEnum.Differential)
                        last = fr.GetLastFull();
                    else if (task.TaskType == ConfigTaskTypeEnum.Incremental)
                        last = fr.GetLast();
                    comparison = _repository.Query<BackupInfo>("backup_info").Include(x => x.Files).Where(x => x.TargetFileName == last.Info.Name).FirstOrDefault();
                }

            }
            result = processor.ProcessTask(task, comparison, nextType);

            _repository.Insert<BackupInfo>(result, "backup_info");
            return Task.CompletedTask;
        }

        private void HandleFileCopied(object sender, FileCopiedEventArgs e)
        {
            _repository.Insert<BackupFile>(e.File, "backup_file");
        }
    }
}

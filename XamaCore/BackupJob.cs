using NLog;
using Quartz;
using System.Threading.Tasks;
using XamaCore.Configs;

namespace XamaCore
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        private Logger _logger => NLog.LogManager.GetCurrentClassLogger();
        
        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task =  data["task"] as ConfigTask;

            _logger.Info($"Starting backup job for {task.Name}");
            return Task.CompletedTask;
        }
    }
}

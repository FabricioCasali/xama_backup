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
        private BackupProcessor _processor;
        private LiteRepository _rep;

        public BackupJob(BackupProcessor bp)
        {
            _processor = bp;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;


            var result = _processor.Process(task);

            return Task.CompletedTask;
        }
    }
}

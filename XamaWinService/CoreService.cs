using System;
using NLog;
using Quartz;
using XamaCore;
using XamaCore.Configs;

namespace XamaWinService
{
    class CoreService
    {
        public CoreService(ConfigApp appConfig, IScheduler scheduler, Logger logger)
        {
            _appConfig = appConfig;
            _scheduler = scheduler;
            _logger = logger;
        }

        private ConfigApp _appConfig;

        private IScheduler _scheduler;

        private Logger _logger;

        public void Start()
        {
            _logger.Info("Service started");
            foreach (var task in _appConfig.Tasks)
            {

                var jobData = new JobDataMap();
                jobData.Add("task", task);
                var job = JobBuilder.Create<BackupJob>()
                    .WithIdentity(task.Name, task.Name)
                    .SetJobData(jobData)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(task.Name, task.Name)
                    .StartNow()
                    .Build();

                _scheduler.ScheduleJob(job, trigger);
                _logger.Info("Task {0} scheduled", task.Name);
            }
            _scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}

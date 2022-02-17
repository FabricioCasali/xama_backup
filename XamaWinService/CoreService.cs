using System;
using System.Linq;
using System.Threading;

using Autofac;

using NLog;

using Quartz;
using Quartz.Impl.Matchers;

using Topshelf;

using XamaWinService.Configs;

namespace XamaWinService
{
    class CoreService : ServiceControl
    {
        public CoreService(ConfigApp appConfig, IScheduler scheduler, ILifetimeScope scope)
        {
            _appConfig = appConfig;
            _scheduler = scheduler;
            _scope = scope;
        }

        private readonly Timer _timer;

        private ConfigApp _appConfig;
        private IScheduler _scheduler;
        private ILifetimeScope _scope;
        private ILogger _logger => LogManager.GetCurrentClassLogger();


        public bool Start(HostControl hostControl)
        {
            _logger.Info("Service started and ready to work");

            _logger.Info("Reading tasks from configuration file and creating the schedules");

            foreach (var task in _appConfig.Tasks.Where(x => x.Schedules != null && x.Schedules.Count > 0))
            {
                var jobData = new JobDataMap();
                jobData.Add("task", task);
                var job = JobBuilder.Create<BackupJob>()
                    .WithIdentity(task.Name, task.Name)
                    .SetJobData(jobData)
                    .Build();

                foreach (var schedule in task.Schedules)
                {
                    var ns = (Schedulers.IScheduler)_scope.ResolveNamed(schedule.GetType().Name, typeof(XamaWinService.Schedulers.IScheduler));
                    //ns.ValidateConfig(schedule);
                    ITrigger trigger;
                    if (!ns.IsCron())
                    {
                        var next = ns.NextRun(DateTime.Now, schedule);
                        _logger.Info($"next execution of task {task.Name} - {schedule.GetType().Name} will be at {next}");
                        trigger = TriggerBuilder.Create()
                           .WithIdentity(Guid.NewGuid().ToString().Replace("-", ""), task.Name)
                           .StartAt(new DateTimeOffset(next))
                           .Build();
                    }
                    else
                    {
                        trigger = TriggerBuilder.Create()
                               .WithIdentity(Guid.NewGuid().ToString().Replace("-", ""), task.Name)
                               .WithCronSchedule(((ConfigScheduleCron)schedule).CronExpression)
                               .Build();
                    }
                    _scheduler.ScheduleJob(job, trigger);
                }
                _logger.Info("Task {0} scheduled", task.Name);
            }
            _scheduler.ListenerManager.AddTriggerListener(new TriggerListener(), GroupMatcher<TriggerKey>.AnyGroup());
            _scheduler.Start();
            return true;
        }

        /// <summary> stop the service </summary>
        public bool Stop(HostControl hostControl)
        {
            _scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
            return true;
        }
    }
}

using System.Threading;
using System.Threading.Tasks;

using NLog;

using Quartz;

namespace XamaWinService
{
    public class TriggerListener : ITriggerListener
    {
        public string Name => "TriggerListener";

        private ILogger _logger => LogManager.GetCurrentClassLogger();

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            _logger.Info("Trigger {0} completed", trigger.Key);
            return Task.CompletedTask;
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                _logger.Info("Trigger {0} fired", trigger.Key);
            });
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                _logger.Info("Trigger {0} misfired", trigger.Key);
            });
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}

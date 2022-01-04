using System;

namespace XamaWinService.Schedulers
{
    public class SchedulerException : Exception
    {
        public SchedulerException(string message) : base(message)
        {
        }
    }
}
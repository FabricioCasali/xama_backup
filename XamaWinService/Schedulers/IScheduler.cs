using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public interface IScheduler
    {
        DateTime NextRun(DateTime baseDate, object config);
        void ValidateConfig(object config);

        bool IsCron();
    }
    public interface IScheduler<T> : IScheduler where T : ConfigSchedule
    {
        DateTime NextRun(DateTime baseDate, T config);

        void ValidateConfig(T config);
    }
}
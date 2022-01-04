using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public interface IScheduler<T> where T : ConfigSchedule
    {
        DateTime NextRun(DateTime baseDate, T config);

        void ValidateConfig(T config);
    }
}
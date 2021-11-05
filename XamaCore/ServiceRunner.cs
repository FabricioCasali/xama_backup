using System;
using NLog;
using XamaCore.Configs;

namespace XamaCore
{
    class CoreService
    {
        public CoreService(ConfigApp appConfig)
        {
            AppConfig = appConfig;
        }

        public ConfigApp AppConfig { get; }

        private Logger _logger => NLog.LogManager.GetCurrentClassLogger();
        public void Start()
        {
            _logger.Info("Service started");
        }

        public void Stop()
        {
            Console.WriteLine("Service is stopping");
        }
    }
}

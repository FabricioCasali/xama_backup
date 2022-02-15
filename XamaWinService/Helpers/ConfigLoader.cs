using System.IO;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using XamaWinService.Configs;

namespace XamaWinService.Helpers
{
    public static class ConfigLoader
    {
        public static ConfigApp ReadConfigFile(String path)
        {
            var jss = new JsonSerializerSettings();
            jss.Converters.Add(new StringEnumConverter());
            jss.Converters.Add(new ScheduleConverter());
            var cfg = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText(path), jss);
            return cfg;
        }
    }
}
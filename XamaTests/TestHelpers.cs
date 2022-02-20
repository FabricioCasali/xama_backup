using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using XamaCore;
using XamaCore.Configs;

using XamaWinService.Configs;
using XamaWinService.Helpers;

namespace XamaTests
{
    public static class TestHelpers
    {
        public static ConfigApp LoadConfig(string path)
        {
            var jss = new JsonSerializerSettings();
            jss.Converters.Add(new StringEnumConverter());
            var cfg = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText(path), jss);
            return cfg;
        }

        public static ConfigApp BuildAndInitializeConfiguration(string testName, string extension, bool generateLog = true)
        {
            var cfg = BuildConfiguration(testName, extension, generateLog);
            if (generateLog)
            {
                NLogInit.Configure(cfg.LogConfig);
            }
            return cfg;
        }

        public static void PrepareRetention(string fileName, int totalFile = 21, int completeEvery = 6)
        {
            var basePath = TestHelpers.RetentionPath();
            var date = new DateTime(2022, 1, 1);

            for (var i = 0; i < totalFile; i++)
            {
                if (i == 0 || i % (completeEvery + 1) == 0)
                {
                    var complete = BackupFileHelper.GetFileName(basePath, fileName, "zip", date.AddDays(i), ConfigTaskTypeEnum.Differential, BackupType.Complete);
                    File.WriteAllText(complete, "test");
                }
                else
                {
                    var n = BackupFileHelper.GetFileName(basePath, fileName, "zip", date.AddDays(i), ConfigTaskTypeEnum.Differential, BackupType.Partial);
                    File.WriteAllText(n, "test");
                }
            }
        }

        public static ConfigApp BuildConfiguration(string testName, string extension, bool generateLog = true)
        {
            var cfg = new ConfigApp();
            if (generateLog)
            {
                cfg.LogConfig = new ConfigLog()
                {
                    LogFileName = $"{testName}.log",
                    LogFilePath = LogsPath(),
                    MaxArchiveFiles = 1,
                    MaxLogSize = 500000,
                    ShowTrace = true
                };
                cfg.EnableLog = true;
            }

            cfg.Tasks = new List<ConfigTask>()
            {
                new ConfigTask()
                {
                    Name = testName,
                    Paths = new List<ConfigPath>()
                    {
                        new ConfigPath()
                        {
                            Path = SourceFilesPath(),
                            IncludeSubfolders = true,
                        }
                    },
                    Target = new ConfigTarget()
                    {
                        Path = TargetPath(),
                        FileName = $"{testName}.{extension}",
                        CompressionLevel = ConfigCompressionLevel.Normal
                    }
                }
            };
            return cfg;
        }

        public static string RetentionPath()
        {
            if (!Directory.Exists(Path.Combine(BasePath(), "retention")))
                Directory.CreateDirectory(Path.Combine(BasePath(), "retention"));

            return Path.Combine(BasePath(), "retention");
        }

        public static string TargetPath()
        {
            if (!Directory.Exists(Path.Combine(BasePath(), "target")))
                Directory.CreateDirectory(Path.Combine(BasePath(), "target"));

            return Path.Combine(BasePath(), "target");
        }

        public static string LogsPath()
        {
            if (!Directory.Exists(Path.Combine(BasePath(), "logs")))
                Directory.CreateDirectory(Path.Combine(BasePath(), "logs"));

            return Path.Combine(BasePath(), "logs");
        }

        public static string SourceFilesPath()
        {
            if (!Directory.Exists(Path.Combine(BasePath(), "sourceFiles")))
                Directory.CreateDirectory(Path.Combine(BasePath(), "sourceFiles"));

            return Path.Combine(BasePath(), "sourceFiles");
        }

        public static string BasePath()
        {
            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "XamaTests")))
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "XamaTests"));

            return Path.Combine(Path.GetTempPath(), "XamaTests");

        }
    }
}


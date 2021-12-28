using ICSharpCode.SharpZipLib.Zip;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XamaCore.Configs;

namespace XamaCore
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        private Logger _logger;

        public BackupJob(Logger logger)
        {
            _logger = logger;
        }

        private String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;

            _logger.Info($"Starting backup job for {task.Name}");
            var outuputPath = Path.Combine(task.Target.Path, task.Target.FileName);

            var start = DateTime.Now;

            using (var os = new ZipOutputStream(File.Create(outuputPath)))
            {
                var numberOfFiles = 0;
                os.SetLevel(9);
                foreach (var p in task.Paths)
                {
                    _logger.Debug($"Backuping {p.Path}");

                    var di = new DirectoryInfo(p.Path);
                    var fls = di.GetFiles("*", p.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
                    foreach (var f in fls)
                    {
                        if (p.Includes != null && p.Includes.Count > 0)
                        {
                            var keep = IsMatch(f.Name, p.Includes);
                            if (!keep)
                            {
                                _logger.Trace($"Skipping {f.FullName} becouse it doesn't match any inclusion rule");
                                continue;
                            }
                            else
                            {
                                _logger.Trace($"Keeping {f.FullName} by inclusion rule");
                            }
                        }

                        if (p.Excludes != null && p.Excludes.Count > 0)
                        {
                            var discard = IsMatch(f.Name, p.Excludes);
                            if (discard)
                            {
                                _logger.Trace($"Discarding {f.FullName} by exclusion rule");
                                continue;
                            }
                        }

                        // todo incluir validações de máscara
                        numberOfFiles++;
                        var entry = new ZipEntry(f.FullName.Substring(di.FullName.Length + 1));
                        entry.DateTime = DateTime.Now;
                        _logger.Debug($"Adding {entry.Name}");
                        os.PutNextEntry(entry);
                        os.WriteAsync(File.ReadAllBytes(f.FullName)).GetAwaiter().GetResult();
                    }
                }
                os.Finish();
                os.Close();
                _logger.Info($"Backup job for {task.Name} finished in {(DateTime.Now - start).TotalSeconds} seconds and backuped {numberOfFiles} files");
            }
            return Task.CompletedTask;
        }

        private bool IsMatch(string fullName, IList<ConfigPattern> rules)
        {
            var isMatch = false;
            foreach (var r in rules)
            {
                // todo rise exception if regex is not valid
                Regex pt;
                if (r.PatternType == ConfigPatternTypeEnum.Name)
                {
                    pt = new Regex(WildCardToRegular(r.Pattern), RegexOptions.IgnoreCase);
                }
                else
                {
                    pt = new Regex(r.Pattern);
                }

                isMatch = pt.IsMatch(fullName);
                if (isMatch)
                {
                    break;
                }
            }
            return isMatch;

        }
    }
}

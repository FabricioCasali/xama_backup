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
                    numberOfFiles = ScanFolder(os, numberOfFiles, p, p.Path);
                }
                os.Finish();
                os.Close();
                _logger.Info($"Backup job for {task.Name} finished in {(DateTime.Now - start).TotalSeconds} seconds and backuped {numberOfFiles} files");
            }
            return Task.CompletedTask;
        }

        /// <summary> check the include/exclude rules and choose de right action </summary>
        /// <param name="p">the path configuration</param>
        /// <param name="f">the info of the file</param>
        private FileAction CheckActionForFile(ConfigPath p, FileSystemInfo f)
        {
            if (p.Includes != null && p.Includes.Count > 0)
            {
                var keep = IsMatch(f, p.Includes);
                if (!keep)
                {
                    _logger.Trace($"Skipping {f.FullName} becouse it doesn't match any inclusion rule");
                    return FileAction.Skip;
                }
                else
                {
                    _logger.Trace($"Keeping {f.FullName} by inclusion rule");
                    return FileAction.Backup;
                }
            }

            if (p.Excludes != null && p.Excludes.Count > 0)
            {
                var discard = IsMatch(f, p.Excludes);
                if (discard)
                {
                    _logger.Trace($"Discarding {f.FullName} by exclusion rule");
                    return FileAction.Skip;
                }
            }
            // if no rule changes the file behavior, the file should be backuped 
            return FileAction.Backup;
        }

        /// <summary> scan folder for file and directories and check if it should be backuped </summary>
        private int ScanFolder(ZipOutputStream os, int numberOfFiles, ConfigPath p, string path)
        {
            var di = new DirectoryInfo(path);
            var fls = di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly).ToList();
            foreach (var f in fls)
            {
                var action = CheckActionForFile(p, f);
                if (action == FileAction.Skip)
                    continue;
                if (f.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (!p.IncludeSubfolders)
                        continue;
                    else
                    {
                        numberOfFiles = ScanFolder(os, numberOfFiles, p, f.FullName);
                        continue;
                    }
                }

                numberOfFiles++;
                var entry = new ZipEntry(f.FullName.Substring(di.FullName.Length + 1));
                entry.DateTime = DateTime.Now;
                _logger.Debug($"Adding {f.FullName} to backup");
                os.PutNextEntry(entry);
                os.WriteAsync(File.ReadAllBytes(f.FullName)).GetAwaiter().GetResult();
            }
            return numberOfFiles;
        }

        /// <summary> check if the file name match any of the rules </summary>
        private bool IsMatch(FileSystemInfo fi, IList<ConfigPattern> rls)
        {
            var isMatch = false;
            foreach (var r in rls)
            {

                if (r.ApplyesTo != ConfigPatternFileType.Both)
                {
                    if ((r.ApplyesTo == ConfigPatternFileType.File && fi.Attributes.HasFlag(FileAttributes.Directory)) ||
                        (r.ApplyesTo == ConfigPatternFileType.Directory && !fi.Attributes.HasFlag(FileAttributes.Directory)))
                        continue;
                }

                Regex pt;
                if (r.PatternType == ConfigPatternTypeEnum.Wildcard)
                {
                    pt = new Regex(WildCardToRegular(r.Pattern), RegexOptions.IgnoreCase);
                }
                else
                {
                    pt = new Regex(r.Pattern);
                }
                var name = fi.Name;
                switch (r.Mode)
                {
                    case ConfigPatternMode.Name:
                        name = Path.GetFileName(fi.FullName);
                        break;
                    case ConfigPatternMode.NameWithoutExtension:
                        name = Path.GetFileNameWithoutExtension(fi.FullName);
                        break;
                    case ConfigPatternMode.FullPath:
                        name = fi.FullName;
                        break;
                }
                isMatch = pt.IsMatch(name);
                if (isMatch)
                {
                    break;
                }
            }
            return isMatch;

        }
    }

    public enum FileAction
    {
        Backup,
        Skip
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using NLog;

using XamaCore.Compressors;
using XamaCore.Configs;
using XamaCore.Data;
using XamaCore.Events;

namespace XamaCore
{

    public class BackupProcessor : IDisposable
    {
        public event EventHandler<FileCopiedEventArgs> FileCopied;
        private ILogger _logger => LogManager.GetCurrentClassLogger();
        private ICompress _compressor;
        private BackupInfo _lastBackup;

        public BackupProcessor(ICompress compressor)
        {
            _compressor = compressor;
        }

        public BackupInfo ProcessTask(ConfigTask t)
        {
            return ProcessTask(t, null, BackupType.Complete);
        }

        /// <summary> initiate the backup process of some task </summary>
        /// <param name="task">the task to be processed</param>
        /// <param name="lastInfo">the last backup of the task</param>
        /// <param name="type">the type of the next backup</param>
        public BackupInfo ProcessTask(ConfigTask task, BackupInfo lastInfo, BackupType type)
        {
            try
            {

                _logger.Info($"Starting backup job for {task.Name}");
                _lastBackup = lastInfo;
                var outputPath = BackupFileHelper.GetFileName(task.Target.Path, task.Target.FileName, _compressor.GetFileExtension(), task.TaskType, type);
                var backupInfo = new BackupInfo();
                _compressor.OpenFile(outputPath, task.Target.CompressionLevel);
                foreach (var configPath in task.Paths)
                {
                    _logger.Debug($"Backuping {configPath.Path}");
                    var basePath = Path.GetFullPath(configPath.Path);
                    ScanFolder(configPath, basePath, backupInfo, basePath);
                }
                _compressor.Close();
                backupInfo.EndedAt = DateTime.Now;
                backupInfo.TotalSize = backupInfo.Files.Sum(x => x.Size);
                var fi = new FileInfo(outputPath);
                backupInfo.TotalCompressedSize = fi.Length;
                backupInfo.TargetFullPath = fi.FullName;
                backupInfo.TargetFileName = fi.Name;
                _logger.Info($"Backup job for {task.Name} finished in {(backupInfo.EndedAt - backupInfo.StartedAt).TotalSeconds} seconds and stored {backupInfo.CopiedFiles} files");
                Clear();
                return backupInfo;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        private void Clear()
        {
            _lastBackup = null;
        }

        /// <summary> fires the event of a new file being copied </summary>
        protected virtual void OnFileCopied(FileCopiedEventArgs e)
        {
            FileCopied?.Invoke(this, e);
        }

        /// <summary> check the include/exclude rules and choose de right action </summary>
        /// <param name="configPath">the path configuration</param>
        /// <param name="fileInfo">the info of the file</param>
        private FileAction CheckActionForFile(ConfigPath configPath, FileSystemInfo fileInfo)
        {
            if (configPath.Includes != null && configPath.Includes.Count > 0)
            {
                if (fileInfo.Attributes.HasFlag(FileAttributes.Directory) && !configPath.Includes.Any(x => x.AppliesTo == ConfigPatternFileType.Directory || x.AppliesTo == ConfigPatternFileType.Both))
                {
                    return FileAction.Backup;
                }
                var keep = IsMatch(fileInfo, configPath.Includes);
                if (!keep)
                {
                    _logger.Trace($"Skipping {fileInfo.FullName} because it doesn't match any inclusion rule");
                    return FileAction.Skip;
                }
                else
                {
                    _logger.Trace($"Keeping {fileInfo.FullName} by inclusion rule");
                }
            }

            if (configPath.Excludes != null && configPath.Excludes.Count > 0)
            {
                var discard = IsMatch(fileInfo, configPath.Excludes);
                if (discard)
                {
                    _logger.Trace($"Discarding {fileInfo.FullName} by exclusion rule");
                    return FileAction.Skip;
                }
            }
            // if no rule changes the file behavior, the file should be part of the backup
            return FileAction.Backup;
        }

        /// <summary> transform a file wildcard pattern (ex: *somefile*.t?t) to a regex pattern </summary>
        private static string WildCardToRegex(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        /// <summary> check if the file name match any of the rules </summary>
        private static bool IsMatch(FileSystemInfo fi, IList<ConfigPattern> rules)
        {
            var isMatch = false;
            foreach (var rule in rules)
            {
                if (rule.AppliesTo != ConfigPatternFileType.Both)
                {
                    if ((rule.AppliesTo == ConfigPatternFileType.File && fi.Attributes.HasFlag(FileAttributes.Directory)) ||
                        (rule.AppliesTo == ConfigPatternFileType.Directory && !fi.Attributes.HasFlag(FileAttributes.Directory)))
                        continue;
                }

                Regex regex;
                if (rule.PatternType == ConfigPatternTypeEnum.Wildcard)
                {
                    regex = new Regex(WildCardToRegex(rule.Pattern), RegexOptions.IgnoreCase);
                }
                else
                {
                    regex = new Regex(rule.Pattern);
                }
                var name = fi.Name;
                switch (rule.Mode)
                {
                    case ConfigPatternMode.Name:
                        name = Path.GetFileName(fi.FullName);
                        break;
                    case ConfigPatternMode.FileNameWithoutExtension:
                        name = Path.GetFileNameWithoutExtension(fi.FullName);
                        break;
                    case ConfigPatternMode.FullPath:
                        name = fi.FullName;
                        break;
                }
                isMatch = regex.IsMatch(name);
                if (isMatch)
                {
                    break;
                }
            }
            return isMatch;
        }

        /// <summary> scan folder for file and directories and check if it should participate in the backup     </summary>
        private void ScanFolder(ConfigPath configPath, string path, BackupInfo backupInfo, string basePath)
        {
            var dirInfo = new DirectoryInfo(path);
            var entries = dirInfo.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly).ToList();
            foreach (var entry in entries)
            {

                if (entry.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (!configPath.IncludeSubfolders)
                        continue;
                    else
                    {
                        var a = CheckActionForFile(configPath, entry);
                        if (a == FileAction.Skip)
                            continue;

                        ScanFolder(configPath, entry.FullName, backupInfo, basePath);
                        continue;
                    }
                }
                var action = CheckActionForFile(configPath, entry);
                if (action == FileAction.Skip)
                    continue;
                var bf = new BackupFile()
                {
                    FullPath = entry.FullName,
                    LastChanged = entry.LastWriteTimeUtc,
                    Name = entry.Name,
                    Size = new FileInfo(entry.FullName).Length,
                };
                backupInfo.Files.Add(bf);
                try
                {
                    bf.MD5 = CalculateChecksum(entry);
                    if (_lastBackup != null)
                    {
                        var old = _lastBackup.Files.FirstOrDefault(x => x.FullPath == bf.FullPath);
                        if (old != null)
                        {
                            if (bf.MD5 == old.MD5)
                            {
                                _logger.Trace($"Skipping {bf.FullPath} because it is the same as the last backup");
                                continue;
                            }
                        }
                    }


                    backupInfo.CopiedFiles++;
                    // fires the event of backup new file 
                    OnFileCopied(new FileCopiedEventArgs(bf));
                    var relativePath = entry.FullName.Substring(basePath.Length);
                    _logger.Debug($"Adding {relativePath} to backup");
                    _compressor.Compress(entry.FullName, relativePath);
                }
                catch (Exception)
                {
                    var p = new BackupProblem()
                    {
                        Description = $"Error while copying {entry.FullName}",
                        ErrorCode = 100,
                        File = bf,
                        IsFatalError = false
                    };
                    if (backupInfo.Problems == null)
                        backupInfo.Problems = new List<BackupProblem>();
                    backupInfo.Problems.Add(p);
                }
            }
        }

        /// <summary> calculate de md5 checksum of file and returns the value </summary>
        private static string CalculateChecksum(FileSystemInfo f)
        {
            string hash = null;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.Open(f.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
            return hash;
        }




        public void Dispose()
        {
            Clear();
        }
    }
}

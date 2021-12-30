using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NLog;
using LiteDB;
using XamaCore.Configs;
using XamaCore.Data;
using Autofac;
using XamaCore.Compressors;

namespace XamaCore
{
    public class BackupProcessor
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();
        private LiteRepository _rep;
        private ILifetimeScope _container;
        private ICompress _compressor;

        public BackupProcessor(LiteRepository rep, ILifetimeScope container)
        {
            _rep = rep;
            _container = container;
        }

        /// <summary> create a unique name to the backup file </summary>
        private static string GetFileName(string basePath, string name, ConfigCompressionMethod method)
        {
            var extension = "";
            if (method == ConfigCompressionMethod.Zip)
                extension = "7zip";
            else
                extension = "zip";
            var fileName = Path.Combine(basePath, $"{name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.{extension}");
            return fileName;
        }

        public BackupInfo Process(ConfigTask t)
        {
            _logger.Info($"Starting backup job for {t.Name}");
            var outputPath = GetFileName(t.Target.Path, t.Target.FileName, t.Target.CompressionMethod);
            var backupInfo = new BackupInfo();
            _rep.Insert<BackupInfo>(backupInfo, "backup_info");
            var compressMethod = "zip";
            if (t.Target.CompressionMethod == ConfigCompressionMethod.SevenZip)
                compressMethod = "7zip";
            _compressor = _container.ResolveNamed<ICompress>(compressMethod);
            _compressor.OpenFile(outputPath, t.Target.CompressionLevel);

            foreach (var configPath in t.Paths)
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
            backupInfo.TargetFileName = fi.FullName;
            _rep.Update<BackupInfo>(backupInfo, "backup_info");
            _logger.Info($"Backup job for {t.Name} finished in {(backupInfo.EndedAt - backupInfo.StartedAt).TotalSeconds} seconds and stored {backupInfo.NumberOfFiles} files");

            return backupInfo;
        }

        /// <summary> check the include/exclude rules and choose de right action </summary>
        /// <param name="configPath">the path configuration</param>
        /// <param name="fileInfo">the info of the file</param>
        private FileAction CheckActionForFile(ConfigPath configPath, FileSystemInfo fileInfo)
        {
            if (configPath.Includes != null && configPath.Includes.Count > 0)
            {
                var keep = IsMatch(fileInfo, configPath.Includes);
                if (!keep)
                {
                    _logger.Trace($"Skipping {fileInfo.FullName} because it doesn't match any inclusion rule");
                    return FileAction.Skip;
                }
                else
                {
                    _logger.Trace($"Keeping {fileInfo.FullName} by inclusion rule");
                    return FileAction.Backup;
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
        private static string WildCardToRegular(string value)
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
                    regex = new Regex(WildCardToRegular(rule.Pattern), RegexOptions.IgnoreCase);
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

        /// <summary> scan folder for file and directories and check if it should participate in the backup </summary>
        private void ScanFolder(ConfigPath configPath, string path, BackupInfo backupInfo, string basePath)
        {
            var dirInfo = new DirectoryInfo(path);
            var entries = dirInfo.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly).ToList();
            foreach (var entry in entries)
            {
                var action = CheckActionForFile(configPath, entry);
                if (action == FileAction.Skip)
                    continue;
                if (entry.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (!configPath.IncludeSubfolders)
                        continue;
                    else
                    {
                        ScanFolder(configPath, entry.FullName, backupInfo, basePath);
                        continue;
                    }
                }
                backupInfo.NumberOfFiles++;
                var bf = new BackupFile()
                {
                    FullPath = entry.FullName,
                    LastChanged = entry.LastWriteTimeUtc,
                    Name = entry.Name,
                    Size = new FileInfo(entry.FullName).Length,
                };
                bf.MD5 = CalculateChecksum(entry);
                backupInfo.Files.Add(bf);
                _logger.Trace($"Saving info of file {bf.Name} in db");
                _rep.Insert<BackupFile>(bf, "backup_files");
                var relativePath = entry.FullName.Substring(basePath.Length + 1);
                _logger.Debug($"Adding {relativePath} to backup");
                _compressor.Compress(entry.FullName, relativePath);
            }
        }

        /// <summary> calculate de md5 checksum of file and returns the value </summary>
        private static string CalculateChecksum(FileSystemInfo f)
        {
            string hash = null;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(f.FullName))
                {
                    hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
            return hash;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using XamaCore.Configs;

namespace XamaCore
{
    public class BackupFileHelper
    {
        public const string FULL_BACKUP = "full";
        public const string DIFFERENTIAL_BACKUP = "diff";
        public const string INCREMENTAL_BACKUP = "incr";

        private string _path;
        private string _name;
        private int _full = 0;
        private int _part = 0;

        private int _partSinceLastFull = 0;
        private int _completeToKeep = 0;
        private int _partialToKeep = 0;
        private SortedDictionary<string, BackupArchiveDetail> _files;
        private SortedDictionary<string, BackupArchiveDetail> _filesToRemove;

        /// <summary>
        /// builds  class and configure the paths and settings to be used
        /// </summary>
        /// <param name="path">path to check</param>
        /// <param name="name">name of the file (task name)</param>
        /// <param name="completeToKeep">number of complete files to keep</param>
        /// <param name="completeEvery">number of partials to make between completes</param>
        public BackupFileHelper(string path, string name, int completeToKeep, int completeEvery)
        {
            _path = path;
            _name = Path.GetFileNameWithoutExtension(name);
            _completeToKeep = completeToKeep;
            _partialToKeep = completeEvery;
            _files = new SortedDictionary<string, BackupArchiveDetail>();
            _filesToRemove = new SortedDictionary<string, BackupArchiveDetail>();
        }

        /// <summary>
        /// get the last complete backup found
        /// </summary>
        /// <returns>last full backup</returns>
        public BackupArchiveDetail GetLastFull()
        {
            return _files.Values.LastOrDefault(x => x.BackupType == BackupType.Complete);
        }

        /// <summary>
        /// get the last backup found
        /// </summary>
        /// <returns>last backup</returns>
        public BackupArchiveDetail GetLast()
        {
            return _files.Values.LastOrDefault();
        }


        /// <summary>
        /// builds the file name
        /// </summary>
        /// <param name="basePath">path to output file</param>
        /// <param name="name">name of the file (<see cref="ConfigTask.Name"/>)</param>
        /// <param name="extension">extension of the file. if the task name has no extension, this one will be used</param>
        /// <param name="taskType">type of the task. (<see cref="ConfigTaskTypeEnum"/>)</param>
        /// <param name="nextType">type of the backup. (<see cref="BackupType"/>)</param>
        /// <returns>name of the file</returns>
        public static string GetFileName(string basePath, string name, string extension, ConfigTaskTypeEnum taskType, BackupType nextType)
        {
            return GetFileName(basePath, name, extension, DateTime.Now, taskType, nextType);
        }

        /// <inheritdoc cref="GetFileName(string, string, string, ConfigTaskTypeEnum)"/>
        /// <param name="date">date of the backup job to be used</param>
        public static string GetFileName(string basePath, string name, string extension, DateTime date, ConfigTaskTypeEnum taskType, BackupType nextType)
        {
            var ex = Path.GetExtension(name);
            if (!string.IsNullOrEmpty(extension))
            {
                ex = extension;
            }
            if (ex.StartsWith("."))
                ex = ex.Substring(1);

            var baseName = Path.GetFileNameWithoutExtension(name);
            var type = string.Empty;
            if (nextType == BackupType.Complete)
            {
                type = FULL_BACKUP;
            }
            else if (taskType == ConfigTaskTypeEnum.Incremental)
            {
                type = INCREMENTAL_BACKUP;
            }
            else if (taskType == ConfigTaskTypeEnum.Differential)
            {
                type = DIFFERENTIAL_BACKUP;
            }
            var fileName = Path.Combine(basePath, $"{baseName}_{date.ToString("yyyyMMddHHmmss")}_{type.ToLower()}.{ex}");
            return fileName;
        }

        /// <summary> read all files in the path to check needed actions (clear, next backup type, etc...) </summary>
        public void CheckPath()
        {
            var di = new DirectoryInfo(_path);
            var fs = di.GetFileSystemInfos($"{_name}*");
            foreach (var f in fs.OrderBy(x => x.Name))
            {
                var time = f.Name.Substring(_name.Length + 1, 14);
                var type = f.Name.Substring(_name.Length + 16, 4);

                if (type.Equals(FULL_BACKUP, StringComparison.InvariantCultureIgnoreCase))
                {
                    _full++;
                    _partSinceLastFull = 0;
                    _files.Add($"{time}|{type}", new BackupArchiveDetail(f, BackupType.Complete));
                }
                else
                {
                    _part++;
                    _partSinceLastFull++;
                    _files.Add($"{time}|{type}", new BackupArchiveDetail(f, BackupType.Partial));
                }
            }
            CheckFilesToRemove();
        }

        private void CheckFilesToRemove()
        {
            // !BUG need to check for partials files to, because user can change the amount of partials in the config file
            if (_full > _completeToKeep)
            {
                var amount = _full - _completeToKeep;
                var compList = _files.Where(x => x.Value.BackupType == BackupType.Complete).Take(amount);

                var partList = _files.Where(x => x.Value.BackupType == BackupType.Partial).Take(amount * _partialToKeep);

                var l = compList.Concat(partList).ToDictionary(x => x.Key, x => x.Value);
                foreach (var i in l)
                {
                    _filesToRemove.Add(i.Key, i.Value);
                }
            }
        }

        /// <summary>
        ///  returns the type of the next backup, to be in order with the parameters
        /// </summary>
        /// <returns>type of the next backup</returns>
        public BackupType NextBackupShouldBe()
        {
            if (_full == 0)
                return BackupType.Complete;

            if (_partSinceLastFull >= _partialToKeep)
                return BackupType.Complete;

            return BackupType.Partial;
        }


        /// <summary>
        /// remove old files from the path. files delete from the disk will <b>NOT</b> be delete from the <see cref="Files()"/> list.
        /// </summary>
        public void ClearPath()
        {
            foreach (var file in _filesToRemove)
            {
                File.Delete(file.Value.Info.FullName);
            }
        }


        /// <summary>
        /// builds a list of files to be deleted in order to be ok with retention parameters        
        /// </summary>
        /// <returns>list with <see cref="FileSystemInfo>"/> of the files to remove</returns>
        public IList<FileSystemInfo> FilesToRemove()
        {
            return _filesToRemove.Values.Select(x => x.Info).ToList();
        }

        /// <summary>
        /// get all the files for the task
        /// </summary>
        /// <returns>list of <see cref="FileSystemInfo"/>'s.</returns>
        public IList<FileSystemInfo> Files()
        {
            return _files.Values.Select(x => x.Info).ToList();
        }
    }
}
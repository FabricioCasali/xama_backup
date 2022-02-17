using System;
using System.Collections.Generic;

namespace XamaCore.Data
{
    public class BackupInfo
    {
        public BackupInfo()
        {
            StartedAt = DateTime.Now;
            Files = new List<BackupFile>();
            Problems = new List<BackupProblem>();
            Id = Guid.NewGuid();
        }

        /// <summary> id of this backup </summary>
        public Guid Id { get; set; }

        /// <summary> start time of the backup </summary>
        public DateTime StartedAt { get; set; }

        /// <summary> end time of the backup </summary>
        public DateTime EndedAt { get; set; }

        /// <summary> total amount of files in backup </summary>
        public int CopiedFiles { get; set; }

        /// <summary> total amount of files that matches the include/exclude patterns </summary>
        public int MatchedFiles { get; set; }

        /// <summary> indicates if has some problem during the backup </summary>
        public bool HasProblems
        {
            get
            {
                return Problems.Count > 0;
            }
        }

        /// <summary> total amount of files that integrates the backup </summary>
        public IList<BackupFile> Files { get; set; }

        /// <summary> list of all problems of the backup process </summary>
        public IList<BackupProblem> Problems { get; set; }

        /// <summary> full path of the backup file </summary>
        public string TargetFullPath { get; set; }

        /// <summary>
        /// name and extension of the target file
        /// </summary>
        /// <value></value>
        public string TargetFileName { get; set; }


        /// <summary> total size of all the files of the backup </summary>
        public Int64 TotalSize { get; set; }

        /// <summary> total size of the backup file </summary>
        public Int64 TotalCompressedSize { get; set; }
    }
}
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

        public Guid Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }

        /// <summary> total amount of files in backup </summary>
        public int CopiedFiles { get; set; }

        /// <summary> total amount of files that matches the include/exclude patterns </summary>
        public int MatchedFiles { get; set; }
        public bool HasProblems
        {
            get
            {
                return Problems.Count > 0;
            }
        }
        public IList<BackupFile> Files { get; set; }
        public IList<BackupProblem> Problems { get; set; }
        public string TargetFileName { get; set; }
        public Int64 TotalSize { get; set; }
        public Int64 TotalCompressedSize { get; set; }
    }
}
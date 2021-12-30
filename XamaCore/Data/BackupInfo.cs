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
        public int NumberOfFiles { get; set; }
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
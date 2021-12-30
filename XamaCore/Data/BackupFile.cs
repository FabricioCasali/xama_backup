using System;

namespace XamaCore.Data
{
    public class BackupFile
    {
        public BackupFile()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public Int64 Size { get; set; }

        public string MD5 { get; set; }

        public string FullPath { get; set; }

        public DateTime LastChanged { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
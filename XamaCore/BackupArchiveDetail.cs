using System.IO;

namespace XamaCore
{

    public class BackupArchiveDetail
    {
        public BackupArchiveDetail()
        {

        }

        public BackupArchiveDetail(FileSystemInfo info, BackupType backupType)
        {
            this.Info = info;
            this.BackupType = backupType;
        }

        public FileSystemInfo Info { get; set; }

        public BackupType BackupType { get; set; }
    }
}
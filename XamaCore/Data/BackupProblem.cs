using System;

namespace XamaCore.Data
{
    public class BackupProblem
    {
        public BackupProblem()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public int ErrorCode { get; set; }

        public bool IsFatalError { get; set; }

        public BackupFile File { get; set; }

        public string Description { get; set; }
    }
}
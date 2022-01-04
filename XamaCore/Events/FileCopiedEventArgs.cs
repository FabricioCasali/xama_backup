using System;
using XamaCore.Data;

namespace XamaCore.Events
{
    public class FileCopiedEventArgs : EventArgs
    {
        public FileCopiedEventArgs()
        {

        }
        public FileCopiedEventArgs(BackupFile e)
        {
            File = e;
        }
        public BackupFile File { get; set; }
    }
}

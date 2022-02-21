using System;

namespace XamaCore.Interceptors
{
    public class FileActionEventArgs : EventArgs
    {
        public string FileName { get; set; }
    }
}
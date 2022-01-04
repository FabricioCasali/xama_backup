using System;
using System.IO;
using XamaCore.Configs;

namespace XamaCore.Interceptors
{
    public interface IFileActionInterceptor
    {
        FileAction CheckActionForFile(ConfigPath configPath, FileSystemInfo fileInfo);
    }

    public class FileActionEventArgs : EventArgs
    {
        public string FileName { get; set; }
    }
}
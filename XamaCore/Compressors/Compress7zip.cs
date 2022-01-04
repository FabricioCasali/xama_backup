using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XamaCore.Configs;

namespace XamaCore.Compressors
{
    public class Compress7zip : ICompress
    {
        private SevenZipCompressor _cp;
        private string _fn;
        private IList<string> _files;

        public void Close()
        {
            _cp.CompressFiles(_fn, _files.ToArray());
        }

        public void Compress(string filePath, string relativePath)
        {
            _files.Add(filePath);
        }

        public void OpenFile(string path, ConfigCompressionLevel compressionLevel)
        {
            var dllpath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
            SevenZip.SevenZipBase.SetLibraryPath(dllpath);
            _cp = new SevenZipCompressor()
            {
                ArchiveFormat = OutArchiveFormat.SevenZip,
                DirectoryStructure = true
            };
            switch (compressionLevel)
            {
                case ConfigCompressionLevel.Ultra:
                    _cp.CompressionLevel = CompressionLevel.Ultra;
                    break;
                case ConfigCompressionLevel.High:
                    _cp.CompressionLevel = CompressionLevel.High;
                    break;
                case ConfigCompressionLevel.Normal:
                    _cp.CompressionLevel = CompressionLevel.Normal;
                    break;
                case ConfigCompressionLevel.Low:
                    _cp.CompressionLevel = CompressionLevel.Low;
                    break;
                case ConfigCompressionLevel.None:
                    _cp.CompressionLevel = CompressionLevel.None;
                    break;
                default:
                    _cp.CompressionLevel = CompressionLevel.Normal;
                    break;
            }
            _fn = path;
            _files = new List<string>();
        }
    }
}
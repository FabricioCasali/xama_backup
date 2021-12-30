using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NLog;
using XamaCore.Configs;

namespace XamaCore.Compressors
{
    public class CompressZip : ICompress
    {
        private ZipOutputStream _os;

        private ILogger _logger => LogManager.GetCurrentClassLogger();

        public void Close()
        {
            if (_os == null)
                throw new Exception("ZipOutputStream is not initialized. Call OpenFile() first.");

            _os.Close();
            _os.Finish();
        }

        public void Compress(string filePath, string relativePath)
        {
            if (_os == null)
                throw new Exception("ZipOutputStream is not initialized. Call OpenFile() first.");


            var entry = new ZipEntry(relativePath);
            entry.DateTime = DateTime.Now;

            _os.PutNextEntry(entry);
            _os.WriteAsync(File.ReadAllBytes(filePath)).GetAwaiter().GetResult();
        }

        public void OpenFile(string path, ConfigCompressionLevel compressionLevel)
        {
            _os = new ZipOutputStream(File.Create(path));
            var cl = 9;
            switch (compressionLevel)
            {
                case ConfigCompressionLevel.None:
                    cl = 0;
                    break;
                case ConfigCompressionLevel.Low:
                    cl = 3;
                    break;
                case ConfigCompressionLevel.Normal:
                    cl = 5;
                    break;
                case ConfigCompressionLevel.High:
                    cl = 7;
                    break;
                case ConfigCompressionLevel.Ultra:
                    cl = 9;
                    break;
            }
            _os.SetLevel(cl);
        }
    }
}
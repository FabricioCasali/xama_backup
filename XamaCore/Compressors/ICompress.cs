using XamaCore.Configs;

namespace XamaCore.Compressors
{
    public interface ICompress
    {
        void Compress(string filePath, string relativePath);
        void OpenFile(string path, ConfigCompressionLevel compressionLevel);
        void Close();
    }
}
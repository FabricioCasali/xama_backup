using XamaCore.Configs;

namespace XamaCore.Compressors
{
    public interface ICompress
    {
        void Compress(string filePath, string relativePath);
        void OpenFile(string path, ConfigCompressionLevel compressionLevel);

        /// <summary> returns the default file extension to this compression method. It will  be used if no extension is provided in the config file </summary>
        string GetFileExtension();
        void Close();
    }
}
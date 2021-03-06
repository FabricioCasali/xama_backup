using System;
using System.IO;

using Tynamix.ObjectFiller;

using XamaCore;

namespace XamaTests
{

    public class FileTestContext : IDisposable
    {
        protected Random random;

        public int TotalFiles { get; private set; }

        public Int64 TotalSize { get; private set; }

        public FileTestContext()
        {
            random = new Random();

            if (Directory.Exists(TestHelpers.SourceFilesPath()))
            {
                Directory.Delete(TestHelpers.SourceFilesPath(), true);
            }

            if (Directory.Exists(TestHelpers.TargetPath()))
            {
                Directory.Delete(TestHelpers.TargetPath(), true);
            }

            if (Directory.Exists(TestHelpers.LogsPath()))
            {
                Directory.Delete(TestHelpers.LogsPath(), true);
            }
            // create a file structure to do backup checks.
            CreateFiles(10, TestHelpers.SourceFilesPath(), 0);
            CreateDirectory(3, TestHelpers.SourceFilesPath(), 1, true, 3, true, 10);
        }

        private void CreateDirectory(int count, string basePath, int level, bool createSubFolders = false, int subFolders = 5, bool createFiles = false, int filesCount = 100)
        {
            for (var i = 1; i <= count; i++)
            {
                var path = Path.Combine(basePath, $"folder_lvl_{level}_{i:000}");
                Directory.CreateDirectory(path);
                if (createSubFolders && level < subFolders)
                    CreateDirectory(count, path, level + 1, createSubFolders, subFolders, createFiles, filesCount);
                if (createFiles)
                    CreateFiles(filesCount, path, level);
            }

        }

        private void CreateFiles(int number, string basePath, int level)
        {
            for (var i = 1; i <= number; i++)
            {
                var s = Randomizer<string>.Create(new Lipsum(LipsumFlavor.LoremIpsum, random.Next(1, 100)));
                string path = Path.Combine(basePath, $"file_lvl_{level}_{i:000}.txt");
                File.WriteAllText(path, s);
                TotalFiles++;
                TotalSize += new FileInfo(path).Length;
            }
        }

        public void Dispose()
        {
            random = null;
        }
    }
}

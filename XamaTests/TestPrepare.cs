using System;
using System.IO;
using Tynamix.ObjectFiller;
using XamaCore.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using LiteDB;
using XamaCore.Data;

namespace XamaTests
{
    public class TestPrepare : IDisposable
    {
        public static ConfigApp LoadConfig(string path)
        {
            var jss = new JsonSerializerSettings();
            jss.Converters.Add(new StringEnumConverter());
            var cfg = JsonConvert.DeserializeObject<ConfigApp>(File.ReadAllText(path), jss);
            return cfg;
        }

        private static LiteDatabase InitializerDb()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xama.db");
            var db = new LiteDatabase(path);
            var mapper = BsonMapper.Global;
            mapper.UseLowerCaseDelimiter();
            mapper.Entity<BackupInfo>().Id(x => x.Id).DbRef(x => x.Files, "backup_file").DbRef(x => x.Problems, "backup_problem");
            mapper.Entity<BackupFile>().Id(x => x.Id).Field(x => x.MD5, "md5");
            mapper.Entity<BackupProblem>().Id(x => x.Id);
            return db;
        }

        public static string BasePath;
        protected Random random;

        public static LiteDatabase LiteDB { get; private set; }

        public static LiteRepository LiteRepository { get; private set; }

        public static int TotalFiles { get; private set; }

        public static Int64 TotalSize { get; private set; }

        public TestPrepare()
        {
            LiteDB = InitializerDb();
            LiteRepository = new LiteRepository(LiteDB);
            random = new Random();
            BasePath = Path.Combine(Path.GetTempPath(), "XamaTests");
            if (Directory.Exists(BasePath))
                Directory.Delete(BasePath, true);
            Directory.CreateDirectory(BasePath);

            // create a file structure to do backup checks.
            CreateFiles(10, BasePath, 0);
            CreateDirectory(3, BasePath, 1, true, 3, true, 10);

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

        private void CreateFiles(int filesCount, string bpath, int level)
        {
            for (var i = 1; i <= filesCount; i++)
            {
                var s = Randomizer<string>.Create(new Lipsum(LipsumFlavor.LoremIpsum, random.Next(100, 10000)));
                string path = Path.Combine(bpath, $"file_lvl_{level}_{i:000}.txt");
                File.WriteAllText(path, s);
                TotalFiles++;
                TotalSize += new FileInfo(path).Length;
            }
        }

        public void Dispose()
        {
            Directory.Delete(BasePath, true);
        }
    }
}

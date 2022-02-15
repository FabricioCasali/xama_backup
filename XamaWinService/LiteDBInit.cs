using System;
using System.IO;

using LiteDB;

using XamaCore.Configs;
using XamaCore.Data;

using XamaWinService.Configs;

namespace XamaWinService
{
    public static class LiteDBINit
    {
        public static LiteDatabase Init(ConfigApp cfg)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xama.db");
            var cs = $"Filename={path};connection=shared";
            var db = new LiteDatabase(cs);
            var mapper = BsonMapper.Global;
            mapper.UseLowerCaseDelimiter();
            mapper.Entity<BackupInfo>().Id(x => x.Id).DbRef(x => x.Files, "backup_file").DbRef(x => x.Problems, "backup_problem");
            mapper.Entity<BackupFile>().Id(x => x.Id).Field(x => x.MD5, "md5");
            mapper.Entity<BackupProblem>().Id(x => x.Id);
            return db;
        }
    }
}
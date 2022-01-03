using System;
using System.IO;
using Xunit;
using System.Collections.Generic;
using FluentAssertions;
using XamaCore;
using XamaCore.Compressors;

namespace XamaTests
{
    public class BasicTests : IClassFixture<TestPrepare>
    {
        [Fact]
        public void Backup_CheckBackupExists()
        {
            var config = TestPrepare.LoadConfig(Path.Combine(AppContext.BaseDirectory, "test1.json"));
            config.Should().NotBeNull("config cannot be null");

            // fix the path to the created one
            config.Tasks[0].Paths[0].Path = TestPrepare.BasePath;
            config.LogConfig.LogFilePath = TestPrepare.BasePath;
            config.LogConfig.LogFilePath = "Backup_CheckBackupExists.log";

            // fix the output file
            config.Tasks[0].Target.Path = Path.GetTempPath();
            config.Tasks[0].Target.FileName = "Backup_CheckBackupExists.zip";

            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(TestPrepare.LiteRepository, new CompressZip());
            var result = bp.Process(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");
            result.TotalSize.Should().Be(TestPrepare.TotalSize, "total size should be the same");
            result.NumberOfFiles.Should().Be(TestPrepare.TotalFiles, "number of files should be the same");

            var file = new FileInfo(config.Tasks[0].Target.Path);
        }
    }
}

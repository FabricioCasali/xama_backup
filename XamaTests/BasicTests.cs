using System;
using System.IO;
using Xunit;
using System.Collections.Generic;
using FluentAssertions;
using XamaCore;
using XamaCore.Compressors;
using System.Reflection;
using XamaCore.Configs;

namespace XamaTests
{
    public class BasicTests : IClassFixture<TestContext>
    {
        public BasicTests(TestContext fixture)
        {
            this._fixture = fixture;

        }
        private TestContext _fixture;

        [Fact]
        public void Config_LoadCOnfigurationFile()
        {
            var config = TestHelpers.LoadConfig(Path.Combine(AppContext.BaseDirectory, "test1.json"));
            config.Should().NotBeNull();
        }

        [Fact]
        public void ZipBackup_Simple()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "zip");

            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(_fixture.LiteRepository, new CompressZip());
            var result = bp.Process(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");
            result.TotalSize.Should().Be(_fixture.TotalSize, "total size should be the same");
            result.NumberOfFiles.Should().Be(_fixture.TotalFiles, "number of files should be the same");
            result.TotalCompressedSize.Should().BeLessThan(_fixture.TotalSize);
            var file = new FileInfo(result.TargetFileName);
            file.Exists.Should().BeTrue();
        }

        [Fact]
        public void SevenZipBackup_Simple()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "7zip");

            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(_fixture.LiteRepository, new Compress7zip());
            var result = bp.Process(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");
            result.TotalSize.Should().Be(_fixture.TotalSize, "total size should be the same");
            result.NumberOfFiles.Should().Be(_fixture.TotalFiles, "number of files should be the same");
            result.TotalCompressedSize.Should().BeLessThan(_fixture.TotalSize);
            var file = new FileInfo(result.TargetFileName);
            file.Exists.Should().BeTrue();
        }

        [Fact]
        public void ZipBackup_WithIncludeWildcardPattern()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "zip");
            var path = config.Tasks[0].Paths[0];
            path.Includes = new List<ConfigPattern>();
            path.Includes.Add(new ConfigPattern()
            {
                Pattern = "*010*.*",
                AppliesTo = ConfigPatternFileType.File,
                Mode = ConfigPatternMode.Name,
                PatternType = ConfigPatternTypeEnum.Wildcard
            });

            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(_fixture.LiteRepository, new CompressZip());
            var result = bp.Process(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");

            result.NumberOfFiles.Should().Be(40);
            var file = new FileInfo(result.TargetFileName);
            file.Exists.Should().BeTrue();
        }

        [Fact]
        public void ZipBackup_WithExcludeWildcardPattern()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "zip");
            var path = config.Tasks[0].Paths[0];
            path.Excludes = new List<ConfigPattern>();
            path.Excludes.Add(new ConfigPattern()
            {
                Pattern = "*010*.*",
                AppliesTo = ConfigPatternFileType.File,
                Mode = ConfigPatternMode.Name,
                PatternType = ConfigPatternTypeEnum.Wildcard
            });

            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(_fixture.LiteRepository, new CompressZip());
            var result = bp.Process(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");

            result.NumberOfFiles.Should().Be(360);
            var file = new FileInfo(result.TargetFileName);
            file.Exists.Should().BeTrue();
        }


        [Fact]
        public void ZipBackup_WithIncludeExcludeWildcardPattern()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "zip");
            var path = config.Tasks[0].Paths[0];
            path.Includes = new List<ConfigPattern>();
            path.Includes.Add(new ConfigPattern()
            {
                Pattern = "*lvl_1*.*",
                AppliesTo = ConfigPatternFileType.File,
                Mode = ConfigPatternMode.Name,
                PatternType = ConfigPatternTypeEnum.Wildcard
            });
            path.Excludes = new List<ConfigPattern>();
            path.Excludes.Add(new ConfigPattern()
            {
                Pattern = "*010*.*",
                AppliesTo = ConfigPatternFileType.File,
                Mode = ConfigPatternMode.Name,
                PatternType = ConfigPatternTypeEnum.Wildcard
            });
            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(_fixture.LiteRepository, new CompressZip());
            var result = bp.Process(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");

            result.NumberOfFiles.Should().Be(27);
            var file = new FileInfo(result.TargetFileName);
            file.Exists.Should().BeTrue();
        }
    }
}

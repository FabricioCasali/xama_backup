using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using FluentAssertions;

using XamaCore;
using XamaCore.Compressors;
using XamaCore.Configs;

using Xunit;

namespace XamaTests
{

    public class CoreTests : IClassFixture<FileTestContext>
    {
        public CoreTests(FileTestContext c)
        {
            this._context = c;
        }

        private FileTestContext _context;

        [Fact]
        public void ZipBackup_Simple()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "zip");
            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(new CompressZip());
            var result = bp.ProcessTask(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");
            result.TotalSize.Should().Be(_context.TotalSize, "total size should be the same");
            result.CopiedFiles.Should().Be(_context.TotalFiles, "number of files should be the same");
            result.TotalCompressedSize.Should().BeLessThan(_context.TotalSize);
            var file = new FileInfo(result.TargetFullPath);
            file.Exists.Should().BeTrue();
        }

        [Fact]
        public void SevenZipBackup_Simple()
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "7zip");

            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var bp = new BackupProcessor(new Compress7zip());
            var result = bp.ProcessTask(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");
            result.TotalSize.Should().Be(_context.TotalSize, "total size should be the same");
            result.CopiedFiles.Should().Be(_context.TotalFiles, "number of files should be the same");
            result.TotalCompressedSize.Should().BeLessThan(_context.TotalSize);
            var file = new FileInfo(result.TargetFullPath);
            file.Exists.Should().BeTrue();
        }


        [Fact]
        public void SevenZipBackup_No_File_Dummy()
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "7zip");
            var pattern = new ConfigPattern()
            {
                Pattern = "*.*",
                PatternType = ConfigPatternTypeEnum.Wildcard,
                AppliesTo = ConfigPatternFileType.Both,
            };
            config.Tasks[0].Paths[0].Excludes = new List<ConfigPattern>() { pattern };
            var bp = new BackupProcessor(new Compress7zip());
            var result = bp.ProcessTask(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");
            result.CopiedFiles.Should().Be(0, "number of files should be the same");
            var file = new FileInfo(result.TargetFullPath);
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
            var bp = new BackupProcessor(new CompressZip());
            var result = bp.ProcessTask(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");

            result.CopiedFiles.Should().Be(40);
            var file = new FileInfo(result.TargetFullPath);
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
            var bp = new BackupProcessor(new CompressZip());
            var result = bp.ProcessTask(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");

            result.CopiedFiles.Should().Be(360);
            var file = new FileInfo(result.TargetFullPath);
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
            var bp = new BackupProcessor(new CompressZip());
            var result = bp.ProcessTask(config.Tasks[0]);
            result.Should().NotBeNull("result cannot be null");

            result.CopiedFiles.Should().Be(27);
            var file = new FileInfo(result.TargetFullPath);
            file.Exists.Should().BeTrue();
        }

        [Fact]
        public void ZipBackup_Incremental()
        {
            var config = TestHelpers.BuildAndInitializeConfiguration(MethodBase.GetCurrentMethod().Name, "zip");
            var path = config.Tasks[0].Paths[0];
            path.Includes = new List<ConfigPattern>();
            path.Includes.Add(new ConfigPattern()
            {
                Pattern = "*.*",
                AppliesTo = ConfigPatternFileType.File,
                Mode = ConfigPatternMode.Name,
                PatternType = ConfigPatternTypeEnum.Wildcard
            });
            // TODO this is not good, i think we need to use de autofac container to do the resolve, but for now it works;
            var firstBackup = new BackupProcessor(new CompressZip());
            var firstResult = firstBackup.ProcessTask(config.Tasks[0]);
            var firstFile = firstResult.Files.FirstOrDefault();
            File.WriteAllText(firstFile.FullPath, "CHANGED FILE CONTENT");
            var secondBackup = new BackupProcessor(new CompressZip());
            var secondResult = secondBackup.ProcessTask(config.Tasks[0], firstResult, BackupType.Partial);
            secondBackup.Should().NotBeNull();
            secondResult.CopiedFiles.Should().Be(1);
        }
    }
}

using System.IO;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using XamaCore;
using XamaCore.Configs;

using Xunit;

namespace XamaTests
{
    public class RetentionTest : IClassFixture<RetentionTestContext>
    {

        public RetentionTest(RetentionTestContext c)
        {
            this._context = c;
        }

        private RetentionTestContext _context;


        [Fact]
        public void Retention_21_2_comp_each_6()
        {
            var fileName = MethodBase.GetCurrentMethod().Name.ToLower();
            var path = TestHelpers.RetentionPath();
            TestHelpers.PrepareRetention(fileName, 21, 6);
            var fr = new BackupFileHelper(path, fileName, 2, 6);
            fr.CheckPath();
            fr.Files().Count.Should().Be(21);
            fr.FilesToRemove().Count.Should().Be(7);
            var firstFull = fr.Files().First();
            var lastFull = fr.Files().Last(x => x.Name.Contains("full."));
            fr.FilesToRemove().Should().Contain(firstFull);
            fr.FilesToRemove().Should().NotContain(lastFull);
            fr.ClearPath();
            var files = fr.Files();
            for (var i = 0; i < files.Count; i++)
            {
                var f = files[i];
                File.Exists(f.FullName).Should().Be(i > 6);
            }
            var a = fr.NextBackupShouldBe();
            a.Should().Be(BackupType.Complete);
        }

        [Fact]
        public void Retention_21_1_comp_each_6()
        {
            var fileName = MethodBase.GetCurrentMethod().Name.ToLower();
            var path = TestHelpers.RetentionPath();
            TestHelpers.PrepareRetention(fileName, 21, 6);
            var fr = new BackupFileHelper(path, fileName, 1, 6);
            fr.CheckPath();
            fr.Files().Count.Should().Be(21);
            fr.FilesToRemove().Count.Should().Be(14);
            var firstFull = fr.Files().First();
            var lastFull = fr.Files().Last(x => x.Name.Contains("full."));
            fr.FilesToRemove().Should().Contain(firstFull);
            fr.FilesToRemove().Should().NotContain(lastFull);
            fr.ClearPath();
            var files = fr.Files();
            for (var i = 0; i < files.Count; i++)
            {
                var f = files[i];
                File.Exists(f.FullName).Should().Be(i > 13);
            }
            var a = fr.NextBackupShouldBe();
            a.Should().Be(BackupType.Complete);
        }

        [Fact]
        public void Retention_30_5_comp_each_1()
        {
            var fileName = MethodBase.GetCurrentMethod().Name.ToLower();
            var path = TestHelpers.RetentionPath();
            TestHelpers.PrepareRetention(fileName, 30, 1);
            var fr = new BackupFileHelper(path, fileName, 5, 1);
            fr.CheckPath();
            fr.Files().Count.Should().Be(30);
            fr.FilesToRemove().Count.Should().Be(20);
            var firstFull = fr.Files().First();
            var lastFull = fr.Files().Last(x => x.Name.Contains("full."));
            fr.FilesToRemove().Should().Contain(firstFull);
            fr.FilesToRemove().Should().NotContain(lastFull);
            fr.ClearPath();
            var files = fr.Files();
            for (var i = 0; i < files.Count; i++)
            {
                var f = files[i];
                File.Exists(f.FullName).Should().Be(i > 19);
            }
            var a = fr.NextBackupShouldBe();
            a.Should().Be(BackupType.Complete);
        }

        [Fact]
        public void Retention_29_5_comp_each_1()
        {
            var fileName = MethodBase.GetCurrentMethod().Name.ToLower();
            var path = TestHelpers.RetentionPath();
            TestHelpers.PrepareRetention(fileName, 29, 1);
            var fr = new BackupFileHelper(path, fileName, 5, 1);
            fr.CheckPath();
            fr.Files().Count.Should().Be(29);
            fr.FilesToRemove().Count.Should().Be(20);
            var firstFull = fr.Files().First();
            var lastFull = fr.Files().Last(x => x.Name.Contains("full."));
            fr.FilesToRemove().Should().Contain(firstFull);
            fr.FilesToRemove().Should().NotContain(lastFull);
            fr.ClearPath();
            var files = fr.Files();
            for (var i = 0; i < files.Count; i++)
            {
                var f = files[i];
                File.Exists(f.FullName).Should().Be(i > 19);
            }
            var a = fr.NextBackupShouldBe();
            a.Should().Be(BackupType.Partial);
        }

    }
}
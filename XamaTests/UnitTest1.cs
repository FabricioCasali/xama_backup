using System;
using System.IO;
using Xunit;
using Tynamix.ObjectFiller;

namespace XamaTests
{
    public class UnitTest1 : IClassFixture<TestPrepare>
    {
        [Fact]
        public void Test1()
        {

        }
    }

    public abstract class TestPrepare : IDisposable
    {

        protected string BasePath;

        public TestPrepare()
        {
            BasePath = Path.Combine(Path.GetTempPath(), "XamaTests");
            if (Directory.Exists(BasePath))
                Directory.Delete(BasePath);
            Directory.CreateDirectory(BasePath);

            // create a file structure to do backup checks.
            Randomizer<string>.Create(new Lipsum(LipsumFlavor.LoremIpsum));


        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

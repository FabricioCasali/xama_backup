using System;
using System.IO;

namespace XamaTests
{
    public class RetentionTestContext : IDisposable
    {

        public RetentionTestContext()
        {
            if (Directory.Exists(TestHelpers.RetentionPath()))
                Directory.Delete(TestHelpers.RetentionPath(), true);
        }

        public void Dispose()
        {

        }
    }
}

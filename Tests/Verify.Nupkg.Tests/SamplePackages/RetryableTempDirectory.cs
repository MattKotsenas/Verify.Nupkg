using System.IO.Abstractions;

namespace Verify.Nupkg.Tests;

/// <summary>
/// Helper class that retries deleting a temp directory multiple times. This avoid flakiness when running tests
/// due to anti-virus or other processes locking the directory.
/// </summary>
internal class RetryableTempDirectory : DisposableDirectory
{
    public RetryableTempDirectory(IDirectoryInfo directoryInfo) : base(directoryInfo)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IOException? lastException = null;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    base.Dispose(disposing);
                    return;
                }
                catch (IOException ex)
                {
                    lastException = ex;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

            if (lastException is not null)
            {
                throw new IOException("Failed to delete temp directory after multiple retries.", lastException);
            }
        }
    }

    // Once https://github.com/TestableIO/System.IO.Abstractions.Extensions/pull/63 lands, remove this method
    // and use the updated TestableIO.Abstractions.Extensions package instead.
    internal static string GetRandomTempPath()
    {
        var temp = Path.GetTempPath();
        var fileName = Path.GetRandomFileName();
        return Path.Combine(temp, fileName);
    }
}

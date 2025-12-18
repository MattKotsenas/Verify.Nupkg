using System.IO.Abstractions;

namespace Verify.Nupkg.Tasks.Tests;

/// <summary>
/// Helper class that retries deleting a temp directory multiple times. This avoids flakiness when running tests
/// due to anti-virus or other processes locking the directory.
/// </summary>
internal class RetryableTempDirectory : DisposableDirectory
{
    public RetryableTempDirectory(IDirectoryInfo directoryInfo) : base(directoryInfo)
    {
    }

    protected override void Dispose(bool disposing)
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

    internal static string GetRandomTempPath()
    {
        var temp = Path.GetTempPath();
        var fileName = Path.GetRandomFileName();
        return Path.Combine(temp, fileName);
    }
}

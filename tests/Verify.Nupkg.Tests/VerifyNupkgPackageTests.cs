using Xunit.Abstractions;

namespace Verify.Nupkg.Tests;

public class VerifyNupkgPackageTests : PrebuiltArtifactsTestBase
{
    public VerifyNupkgPackageTests(ITestOutputHelper output) : base(output, "Verify.Nupkg")
    {
    }

    [Fact]
    public Task Baseline()
    {
        // This isn't a test of Verify.Nupkg project, but a baseline for the package itself.
        // i.e. we're using the plugin to baseline its own package.

        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(PackagePath, settings);
    }
}

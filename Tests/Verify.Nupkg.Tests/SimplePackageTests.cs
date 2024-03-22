namespace Verify.Nupkg.Tests;

public class SimplePackageTests
{
    private string _simplePackage = SamplePackages.Instance.SimplePackage.Value.FullName;

    [Fact]
    public Task BasicTest()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_simplePackage, settings);
    }

    [Fact]
    public Task CustomFileExclusionTest()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();
        settings.AddNupkgDiffSettings(settings =>
        {
            settings.ExcludedFiles = [new(@"\.psmdcp$"), new(@"\.nuspec$")];
        });

        return VerifyFile(_simplePackage, settings);
    }

    [Fact]
    public Task OnlyOptInScrubbersRun()
    {
        // In this test we intentionally _do not_ include these scrubbers:
        //  - Version
        //  - Schema
        // to validate that only scrubbers we opt-in to are applied.

        VerifySettings settings = new();
        settings.UseUniqueDirectory();

        settings.ScrubNuspecCommit();
        settings.ScrubNuspecRepositoryUrl();

        return VerifyFile(_simplePackage, settings);
    }
}

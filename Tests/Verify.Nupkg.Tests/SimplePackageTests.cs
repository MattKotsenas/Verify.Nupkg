namespace Verify.Nupkg.Tests;

[TestClass]
[UsesVerify]
public partial class SimplePackageTests
{
    private string _simplePackage = SamplePackages.Instance.SimplePackage.Value.FullName;

    [TestMethod]
    public Task BasicTest()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_simplePackage, settings);
    }

    [TestMethod]
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

    [TestMethod]
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

namespace Verify.Nupkg.Tests;

[TestClass]
public partial class SnupkgTests
{
    private readonly string _snupkg = SamplePackages.Instance.SimplePackageWithSymbols.Value.Snupkg.FullName;

    [TestMethod]
    public Task BasicSnupkgTest()
    {
        return VerifyFile(_snupkg).ScrubNuspec();
    }

    [TestMethod]
    public Task SnupkgWithCustomFileExclusionTest()
    {
        return VerifyFile(_snupkg)
            .ScrubNuspec()
            .AddNupkgDiffSettings(settings => settings.ExcludedFiles = [new(@"\.psmdcp$"), new(@"\.nuspec$")]);
    }

    [TestMethod]
    public Task SnupkgWithOnlyOptInScrubbersRun()
    {
        // In this test we intentionally _do not_ include these scrubbers:
        //  - Version
        //  - Schema
        // to validate that only scrubbers we opt-in to are applied.
        return VerifyFile(_snupkg)
            .ScrubNuspecCommit()
            .ScrubNuspecRepositoryUrl()
            .ScrubNuspecBranch();
    }
}

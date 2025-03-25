namespace Verify.Nupkg.Tests;

[TestClass]
public partial class SimplePackageTests
{
    private readonly string _simplePackage = SamplePackages.Instance.SimplePackage.Value.FullName;

    [TestMethod]
    public Task BasicTest()
    {
        return VerifyFile(_simplePackage).ScrubNuspec();
    }

    [TestMethod]
    public Task CustomFileExclusionTest()
    {
        return VerifyFile(_simplePackage)
            .ScrubNuspec()
            .AddNupkgDiffSettings(settings => settings.ExcludedFiles = [new(@"\.psmdcp$"), new(@"\.nuspec$")]);
    }

    [TestMethod]
    public Task OnlyOptInScrubbersRun()
    {
        // In this test we intentionally _do not_ include these scrubbers:
        //  - Version
        //  - Schema
        // to validate that only scrubbers we opt-in to are applied.
        return VerifyFile(_simplePackage)
            .ScrubNuspecCommit()
            .ScrubNuspecRepositoryUrl()
            .ScrubNuspecBranch();
    }
}

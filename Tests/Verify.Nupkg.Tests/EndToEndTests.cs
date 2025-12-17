namespace Verify.Nupkg.Tests;

[TestClass]
public partial class EndToEndTests
{
    private readonly string _nupkg = SamplePackages.Instance.SimplePackageWithSymbols.Value.Nupkg.FullName;
    private readonly string _snupkg = SamplePackages.Instance.SimplePackageWithSymbols.Value.Snupkg.FullName;


    [TestMethod]
    public Task SimpleNupkg()
    {
        return VerifyFile(_nupkg).ScrubNuspec();
    }

    [TestMethod]
    public Task CustomFileExclusions()
    {
        return VerifyFile(_nupkg)
            .ScrubNuspec()
            .AddNupkgDiffSettings(settings => settings.ExcludedFiles = [new(@"\.psmdcp$"), new(@"\.nuspec$")]);
    }

    [TestMethod]
    public Task SimpleSnupkg()
    {
        return VerifyFile(_snupkg).ScrubNuspec();
    }
}

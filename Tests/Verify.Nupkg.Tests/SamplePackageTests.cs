namespace Verify.Nupkg.Tests;

public class SamplePackageTests
{
    private string _package = SamplePackages.Instance.SimplePackage.Value.FullName;

    [Fact]
    public Task BasicTest()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_package, settings);
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

        return VerifyFile(_package, settings);
    }
}
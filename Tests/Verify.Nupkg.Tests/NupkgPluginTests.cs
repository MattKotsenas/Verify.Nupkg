using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Verify.Nupkg.Tests;

public class NupkgPluginTests : TestBase
{
    public NupkgPluginTests(ITestOutputHelper output) : base(output, "SamplePackage")
    {
    }

    [Fact]
    public Task BasicTest()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(PackagePath, settings);
    }

    [Fact]
    public Task CustomFileExclusionTest()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();
        settings.AddNupkgDiffSettings(settings =>
        {
            settings.ExcludedFiles = new List<Regex>
            {
                new(@"\.psmdcp$"),
                new(@"\.nuspec$")
            };
        });

        return VerifyFile(PackagePath, settings);
    }
}
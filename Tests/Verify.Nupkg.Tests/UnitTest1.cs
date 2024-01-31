using Xunit.Abstractions;

namespace Verify.Nupkg.Tests;

public abstract class TestsBase
{
    protected ITestOutputHelper Output { get; private set; }
    protected string ArtifactsPath { get; private set; }
    protected Uri[] PackageFeeds { get; private set; }
    protected string PackageName { get; private set; } = "SamplePackage";
    protected string PackageVersion { get; private set; }
    protected string PackagePath { get; private set; }

    protected TestsBase(ITestOutputHelper output)
    {
        Output = output;

        ArtifactsPath = RetrieveArtifactsPathFromAssemblyMetadata();
        PackageFeeds =
        [
            ConvertArtifactsPathToNuGetFeedUri(),
            new Uri("https://api.nuget.org/v3/index.json")
        ];
        var pack = GetLatestPackageVersion();

        PackageVersion = pack.Version;
        PackagePath = pack.package.FullName;
    }

    private string RetrieveArtifactsPathFromAssemblyMetadata()
    {
        string packagePath = new PackagePathLocator().Locate();

        return packagePath;
    }

    private Uri ConvertArtifactsPathToNuGetFeedUri()
    {
        return NupkgFinder.Find(ArtifactsPath).AsFeedUri();
    }

    private (FileInfo package, string Version) GetLatestPackageVersion()
    {
        var package = NupkgFinder.Find(ArtifactsPath).LatestWithName(PackageName);

        return package;
    }
}

public class HelloWorldTest : TestsBase
{
    public HelloWorldTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void HelloWorld()
    {
        Assert.NotNull(PackagePath);
    }
}
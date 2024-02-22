using Xunit.Abstractions;

namespace Verify.Nupkg.Tests;

public abstract class PrebuiltArtifactsTestBase
{
    protected ITestOutputHelper Output { get; private set; }
    protected string PackagePath { get; private set; }

    protected PrebuiltArtifactsTestBase(ITestOutputHelper output, string packageName)
    {
        Output = output;

        string artifactsPath = RetrieveArtifactsPathFromAssemblyMetadata();
        PackagePath = GetLatestPackageVersion(artifactsPath, packageName).FullName;
    }

    private static string RetrieveArtifactsPathFromAssemblyMetadata()
    {
        string packagePath = new PackagePathLocator().Locate();

        return packagePath;
    }

    private static FileInfo GetLatestPackageVersion(string artifactsPath, string packageName)
    {
        return NupkgFinder.Find(artifactsPath).LatestWithName(packageName).Package;
    }
}

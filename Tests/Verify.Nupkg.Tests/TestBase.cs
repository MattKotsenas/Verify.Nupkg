using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Verify.Nupkg.Tests;

public abstract class TestBase
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyNupkg.Initialize();
    }

    protected ITestOutputHelper Output { get; private set; }
    protected string PackagePath { get; private set; }

    protected TestBase(ITestOutputHelper output, string packageName)
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

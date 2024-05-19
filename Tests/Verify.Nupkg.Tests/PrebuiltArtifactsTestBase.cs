using System.Reflection;

namespace Verify.Nupkg.Tests;

public abstract class PrebuiltArtifactsTestBase
{
    protected string PackagePath { get; private set; }

    protected PrebuiltArtifactsTestBase(string packageName)
    {
        PackagePath = GetLatestPackageVersion(packageName).FullName;
    }

    private static FileInfo GetLatestPackageVersion(string packageName)
    {
        string assemblyLocation = GetAssemblyLocation(Assembly.GetExecutingAssembly());
        return new FileInfo(assemblyLocation)
            .Directory!
            .GetFiles($"{packageName}*.nupkg")
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .First();
    }

    private static string GetAssemblyLocation(Assembly assembly)
    {
#if NETFRAMEWORK
        Uri codebase = new(assembly.CodeBase);
        return Uri.UnescapeDataString(codebase.AbsolutePath);
#elif NETCOREAPP
        return assembly.Location;
#endif
        throw new InvalidOperationException("Unsupported framework.");
    }
}

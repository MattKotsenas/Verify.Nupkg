using System.Reflection;

namespace Verify.Nupkg.Tests;

internal class PackagePathLocator
{
    private readonly Assembly _assembly;

    public PackagePathLocator() : this(typeof(PackagePathLocator).Assembly)
    {
    }


    public PackagePathLocator(Assembly assembly)
    {
        _assembly = assembly;
    }

    public string Locate()
    {
        // Disable nullable warnings for this method because the assembly metadata for .ToDictionary()
        // is <string, string> and in net472, but <string, string?> in net8.0.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        IReadOnlyDictionary<string, string?> metadata = _assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToDictionary(a => a.Key, a => a.Value);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        if (!metadata.TryGetValue("ArtifactsPath", out string? artifactsPath))
        {
            throw new Exception("Unable to get assembly metadata 'ArtifactsPath'");
        }

        if (artifactsPath is null)
        {
            throw new Exception("Assembly metadata 'ArtifactsPath' is null.");
        }

        if (!Directory.Exists(artifactsPath))
        {
            throw new Exception($"Artifacts path '{artifactsPath}' does not exist.");
        }

        if (!metadata.TryGetValue("BuildConfiguration", out string? buildConfiguration))
        {
            throw new Exception("Unable to get assembly metadata 'BuildConfiguration'");
        }

        if (buildConfiguration is null)
        {
            throw new Exception("Assembly metadata 'BuildConfiguration' is null.");
        }

        string result = Path.Combine(artifactsPath, "package", buildConfiguration.ToLowerInvariant()); // Build configuration is normalized to lower case when using artifacts

        if (!Directory.Exists(result))
        {
            throw new Exception($"Package path '{result}' does not exist.");
        }

        return result;
    }
}

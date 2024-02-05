namespace Verify.Nupkg.Tests;

/// <summary>
/// Extension methods to help find .nupkg files under a given root directory.
/// </summary>
public static class NupkgFinder
{
    public static IReadOnlyCollection<FileInfo> Find(string root)
    {
        string[] packages = Directory.GetFiles(root, "*.nupkg", SearchOption.AllDirectories);

        return packages.Select(p => new FileInfo(p)).ToArray();
    }

    public static Uri AsFeedUri(this IEnumerable<FileInfo> packages)
    {
        string[] directories = packages.Select(p => p.Directory!.FullName).Distinct().ToArray();

        return new Uri(directories.Single());
    }

    public static (FileInfo Package, string Version) LatestWithName(this IEnumerable<FileInfo> packages, string name)
    {
        FileInfo package = packages
            .Where(p => p.Name.StartsWith($"{name}."))
            .OrderByDescending(p => p.LastAccessTimeUtc)
            .First();

        return (package, package.GetNuGetPackageVersion(name));
    }

    public static string GetNuGetPackageVersion(this FileInfo package, string name)
    {
        return package.Name.Replace($"{name}.", string.Empty).Replace(package.Extension, string.Empty);
    }
}

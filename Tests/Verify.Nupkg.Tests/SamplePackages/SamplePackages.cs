using System.IO.Abstractions;
using System.Reflection;

namespace Verify.Nupkg.Tests;

internal class SamplePackages
{
    private readonly IFileSystem _fs = new FileSystem();
    public static SamplePackages Instance { get; } = new();

    public Lazy<IFileInfo> SimplePackage { get; private set; }
    public Lazy<IFileInfo> PackageWithRepoGitExtension { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoGitExtension { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoHttps { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoGitHubDomain { get; private set; }

    private SamplePackages()
    {
        IDirectoryInfo workingDirectory = GetWorkingDirectory();

        SimplePackage = new Lazy<IFileInfo>(() => new SimplePackage().Create(workingDirectory));
        PackageWithRepoGitExtension = new Lazy<IFileInfo>(() => new PackageWithRepoGitExtension().Create(workingDirectory));
        PackageWithoutRepoGitExtension = SimplePackage;
        PackageWithoutRepoHttps = new Lazy<IFileInfo>(() => new PackageWithoutRepoHttps().Create(workingDirectory));
        PackageWithoutRepoGitHubDomain = new Lazy<IFileInfo>(() => new PackageWithoutRepoGitHubDomain().Create(workingDirectory));
    }

    private IDirectoryInfo GetWorkingDirectory()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        var dirInfo = _fs.FileInfo.New(assembly.Location).Directory ?? throw new Exception($"Unable to get directory from assembly location '{assembly.Location}'.");

        return dirInfo;
    }
}

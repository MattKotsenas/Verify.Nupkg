using System.IO.Abstractions;
using System.Reflection;

namespace Verify.Nupkg.Tests;

// The idea of this class is to create a single set of sample packages that can be used
// in tests, and that are created only once. Using the package classes directly, without
// the `Lazy<T>` might result in MSBuild getting called multiple times.
internal class SamplePackages
{
    private readonly IFileSystem _fs = new FileSystem();
    public static SamplePackages Instance { get; } = new();

    public Lazy<IFileInfo> SimplePackage { get; private set; }
    public Lazy<IFileInfo> PackageWithRepoGitExtension { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoGitExtension { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoHttps { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoGitHubDomain { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoUrl { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoCommit { get; private set; }
    public Lazy<IFileInfo> PackageWithoutRepoBranch { get; private set; }

    private SamplePackages()
    {
        IDirectoryInfo workingDirectory = GetWorkingDirectory();

        SimplePackage = new Lazy<IFileInfo>(() => new SimplePackage().Create(workingDirectory));
        PackageWithRepoGitExtension = new Lazy<IFileInfo>(() => new PackageWithRepoGitExtension().Create(workingDirectory));
        PackageWithoutRepoGitExtension = SimplePackage;
        PackageWithoutRepoHttps = new Lazy<IFileInfo>(() => new PackageWithoutRepoHttps().Create(workingDirectory));
        PackageWithoutRepoGitHubDomain = new Lazy<IFileInfo>(() => new PackageWithoutRepoGitHubDomain().Create(workingDirectory));
        PackageWithoutRepoUrl = new Lazy<IFileInfo>(() => new PackageWithoutRepoUrlOrCommitOrBranch().Create(workingDirectory));
        PackageWithoutRepoCommit = new Lazy<IFileInfo>(() => new PackageWithoutRepoUrlOrCommitOrBranch().Create(workingDirectory));
        PackageWithoutRepoBranch = new Lazy<IFileInfo>(() => new PackageWithoutRepoUrlOrCommitOrBranch().Create(workingDirectory));
    }

    private IDirectoryInfo GetWorkingDirectory()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        var dirInfo = _fs.FileInfo.New(assembly.Location).Directory ?? throw new Exception($"Unable to get directory from assembly location '{assembly.Location}'.");

        return dirInfo;
    }
}

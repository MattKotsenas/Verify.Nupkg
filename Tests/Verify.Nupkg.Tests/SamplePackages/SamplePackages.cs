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

    public Lazy<(IFileInfo Nupkg, IFileInfo Snupkg)> SimplePackageWithSymbols { get; private set; }

    private SamplePackages()
    {
        IDirectoryInfo workingDirectory = GetWorkingDirectory();

        SimplePackageWithSymbols = new Lazy<(IFileInfo, IFileInfo)>(() => new SimplePackageWithSymbols().CreateWithSymbols(workingDirectory));
    }

    private IDirectoryInfo GetWorkingDirectory()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        var dirInfo = _fs.FileInfo.New(assembly.Location).Directory ?? throw new Exception($"Unable to get directory from assembly location '{assembly.Location}'.");

        return dirInfo;
    }
}

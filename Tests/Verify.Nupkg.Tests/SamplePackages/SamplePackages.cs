using System.IO.Abstractions;
using System.Reflection;

namespace Verify.Nupkg.Tests;

internal class SamplePackages
{
    private readonly IFileSystem _fs = new FileSystem();

    public Lazy<IFileInfo> SimplePackage { get; private set; }
    public static SamplePackages Instance { get; } = new();

    private SamplePackages()
    {
        IDirectoryInfo workingDirectory = GetWorkingDirectory();

        SimplePackage = new Lazy<IFileInfo>(() => new SimplePackage().Create(workingDirectory));
    }

    private IDirectoryInfo GetWorkingDirectory()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        var dirInfo = _fs.FileInfo.New(assembly.Location).Directory ?? throw new Exception($"Unable to get directory from assembly location '{assembly.Location}'.");

        return dirInfo;
    }
}

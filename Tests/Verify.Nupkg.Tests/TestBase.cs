using Microsoft.Build.Execution;
using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;
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

public abstract class TestBase2 : MSBuildTestBase
{
    // TODO: Fix this
    //[ModuleInitializer]
    //public static void Initialize()
    //{
    //    VerifyNupkg.Initialize();
    //}

    protected string PackagePath { get; private set; }

    private Stack<IDisposable> _disposables = new Stack<IDisposable>();

    protected TestBase2()
    {
        PackagePath = CreatePackage();
    }

    private string CreatePackage()
    {
        string resultFile = Directory.GetCurrentDirectory() + "\\SamplePackage.nupkg";

        IFileSystem fs = new FileSystem();

        _disposables.Push(fs.CreateDisposableDirectory(out IDirectoryInfo baseDir));
        _disposables.Push(PackageRepository.Create(baseDir.FullName, feeds: new Uri("https://api.nuget.org/v3/index.json")));

        File.WriteAllText(Path.Combine(baseDir.FullName, "README.md"), "Hello World!");

        ProjectCreator.Templates.SdkCsproj()
            .Property("TargetFramework", "net8.0")
            .Property("PackageReadmeFile", "README.md")
            .Property("RepositoryType", "git")
            .Property("RepositoryUrl", "https://github.com/MattKotsenas/Verify.Nupkg")
            .Property("RepositoryCommit", "0e4d1b598f350b3dc675018d539114d1328189ef ")
            .ItemNone(include: "README.md", metadata: new Dictionary<string, string?>
            {
                { "Pack", "true" },
                { "PackagePath", "\\" },
            })
            .Target(name: "CopyPackageForTests", afterTargets: "Pack")
                .Task(name: "Copy", parameters: new Dictionary<string, string?>
                {
                    { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                    { "DestinationFiles", resultFile },
                })
            .Save(Path.Combine(baseDir.FullName, "SamplePackage.csproj"))
            .TryBuild(restore: true, target: "Pack", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

        if (!result)
        {
            throw new Exception($"Failed to build in path '{baseDir.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
        }

        Dispose();

        return resultFile;
    }

    // TODO: Fix this
    public void Dispose()
    {
        // Clean up
        while (_disposables.Count > 0)
        {
            _disposables.Pop().Dispose();
        }
    }
}

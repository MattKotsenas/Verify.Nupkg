using Microsoft.Build.Execution;
using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;

namespace Verify.Nupkg.Tests;

internal abstract class PackageCreator
{
    private readonly IFileSystem _fs = new FileSystem();

    public abstract string Name { get; }

    protected abstract ProjectCreator CreateCore(IDirectoryInfo workingDirectory);

    public IFileInfo Create(IDirectoryInfo workingDirectory)
    {
        IFileInfo targetPackage = _fs.FileInfo.New(_fs.Path.Combine(workingDirectory.FullName, $"{Name}.nupkg"));

        using (_fs.CreateDisposableDirectory(out IDirectoryInfo baseDir))
        {
            using (PackageRepository.Create(baseDir.FullName, feeds: new Uri("https://api.nuget.org/v3/index.json")))
            {
                CreateCore(baseDir)
                    .Target(name: "CopyPackageForTests", afterTargets: "Pack")
                        .Task(name: "Copy", parameters: new Dictionary<string, string?>
                        {
                            { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                            { "DestinationFiles", targetPackage.FullName },
                        })
                    .Save(_fs.Path.Combine(baseDir.FullName, $"{Name}.csproj"))
                    .TryBuild(restore: true, target: "Pack", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

                if (!result)
                {
                    throw new Exception($"Failed to build in path '{baseDir.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
                }
            }
        }

        return targetPackage;
    }
}

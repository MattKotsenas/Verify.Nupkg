using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;

namespace Verify.Nupkg.Tests;

internal class PackageWithoutRepoHttps : PackageCreator
{
    public override string Name => GetType().Name;

    protected override ProjectCreator CreateCore(IDirectoryInfo workingDirectory)
    {
        return ProjectCreator.Templates.SdkCsproj()
            .Property("TargetFramework", "net8.0")
            .Property("RepositoryType", "git")
            .Property("RepositoryUrl", "http://github.com/MattKotsenas/Verify.Nupkg")
            .Property("RepositoryCommit", "0e4d1b598f350b3dc675018d539114d1328189ef");
    }
}

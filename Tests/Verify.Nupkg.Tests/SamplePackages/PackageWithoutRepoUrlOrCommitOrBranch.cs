using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;

namespace Verify.Nupkg.Tests;

internal class PackageWithoutRepoUrlOrCommitOrBranch : PackageCreator
{
    public override string Name => GetType().Name;

    protected override ProjectCreator CreateCore(IDirectoryInfo workingDirectory)
    {
        return ProjectCreator.Templates.SdkCsproj()
            .Property("TargetFramework", "net8.0")
            .Property("RepositoryType", "git");
    }
}

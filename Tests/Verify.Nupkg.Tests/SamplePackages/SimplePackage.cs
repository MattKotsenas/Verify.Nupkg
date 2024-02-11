using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;

namespace Verify.Nupkg.Tests;

internal class SimplePackage : PackageCreator
{
    public override string Name => nameof(SimplePackage);

    protected override ProjectCreator CreateCore(IDirectoryInfo workingDirectory)
    {
        File.WriteAllText(Path.Combine(workingDirectory.FullName, "README.md"), "Hello, World!");

        return ProjectCreator.Templates.SdkCsproj()
            .Property("TargetFramework", "net8.0")
            .Property("PackageReadmeFile", "README.md")
            .Property("RepositoryType", "git")
            .Property("RepositoryUrl", "https://github.com/MattKotsenas/Verify.Nupkg")
            .Property("RepositoryCommit", "0e4d1b598f350b3dc675018d539114d1328189ef ")
            .ItemNone(include: "README.md", metadata: new Dictionary<string, string?>
            {
                { "Pack", "true" },
                { "PackagePath", "\\" },
            });
    }
}

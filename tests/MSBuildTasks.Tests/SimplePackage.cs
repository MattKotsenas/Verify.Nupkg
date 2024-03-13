using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;

namespace MSBuildTasks.Tests;

// TODO: Move to shared project

internal class SimplePackage : PackageCreator
{
    public override string Name => GetType().Name;

    protected override ProjectCreator CreateCore(IDirectoryInfo workingDirectory)
    {
        return ProjectCreator.Templates.SdkCsproj()
            .Property("TargetFramework", "net8.0")
            ;
    }
}

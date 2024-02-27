using Microsoft.Build.Utilities.ProjectCreation;
using System.Runtime.CompilerServices;

namespace MSBuildTasks.Tests;

internal class ModuleInitializer : MSBuildTestBase
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Initialize the MSBuild.ProjectCreation package
        _ = new ModuleInitializer();
    }
}

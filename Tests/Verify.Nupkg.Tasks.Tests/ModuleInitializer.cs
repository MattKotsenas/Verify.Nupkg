using System.Runtime.CompilerServices;

using Microsoft.Build.Utilities.ProjectCreation;

namespace Verify.Nupkg.Tasks.Tests;

internal class ModuleInitializer : MSBuildTestBase
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Initialize the MSBuild.ProjectCreation package
        _ = new ModuleInitializer();
    }
}

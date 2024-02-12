using Microsoft.Build.Utilities.ProjectCreation;
using System.Runtime.CompilerServices;

namespace Verify.Nupkg.Tests;

internal class ModuleInitializer : MSBuildTestBase
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Initialize the Verify.Nupkg package
        VerifyNupkg.Initialize();

        // Initialize the MSBuild.ProjectCreation package
        _ = new ModuleInitializer();
    }
}

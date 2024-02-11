using Microsoft.Build.Utilities.ProjectCreation;
using System.Runtime.CompilerServices;

namespace Verify.Nupkg.Tests;

internal class ModuleInitializer : MSBuildTestBase
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyNupkg.Initialize();

        _ = new ModuleInitializer();
    }
}

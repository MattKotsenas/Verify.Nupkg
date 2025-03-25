using System.Runtime.CompilerServices;

namespace Verify.Nupkg.Tasks;

public class VerifyModuleInitializer
{
#pragma warning disable CA2255 // The ModuleInitializer attribute should not be used in libraries
    [ModuleInitializer] // Required by Verify
#pragma warning restore CA2255 // The ModuleInitializer attribute should not be used in libraries
    public static void Initialize() => VerifyNupkg.Initialize();
}

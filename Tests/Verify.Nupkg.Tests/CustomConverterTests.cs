using VerifyTests;
using Xunit.Abstractions;

namespace Verify.Nupkg.Tests;

public class CustomConverterTests : TestBase
{
    public CustomConverterTests(ITestOutputHelper output) : base(output, "SamplePackage")
    {
    }

    [Fact]
    public Task CustomConverterIsUsed()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(PackagePath, settings);
    }
}
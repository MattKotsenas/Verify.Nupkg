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
        return VerifyFile(PackagePath);
    }
}
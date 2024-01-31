# Verify.Nupkg

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of [NuGet .nupkg](https://learn.microsoft.com/en-us/nuget/what-is-nuget) files.

## Usage

```csharp
[ModuleInitializer]
public static void Initialize() =>
    VerifyNupkg.Initialize();
```

### File path

```csharp
[Fact]
public Task VerifyNupkgFile()
{
    string packagePath = "path/to/package.nupkg";

    VerifySettings settings = new();
    settings.UseUniqueDirectory(); // Optional; group files into a directory
    settings.ScrubNuspec(); // Scrub commit and other volatile information from nuspec

    return VerifyFile(packagePath, settings);
}
```

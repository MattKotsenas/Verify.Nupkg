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

    return Verify(packagePath);
}
```

![Icon](https://raw.githubusercontent.com/MattKotsenas/Verify.Nupkg/main/icon.png)

# Verify.Nupkg

[![Build status](https://github.com/MattKotsenas/Verify.Nupkg/actions/workflows/main.yml/badge.svg)](https://github.com/MattKotsenas/Verify.Nupkg/actions/workflows/main.yml)
![Nuget](https://img.shields.io/nuget/v/Verify.Nupkg)
[![Downloads](https://img.shields.io/nuget/dt/Verify.Nupkg)](https://nuget.org/packages/Verify.Nupkg)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of [NuGet .nupkg](https://learn.microsoft.com/en-us/nuget/what-is-nuget) files.

The plugin does not do a naive binary comparison, as that would cause a large amount of verification churn. Instead,
the contents of the .nuspec file are verified, along with a tree view of the package files.

Here's an example of the diff that results from adding a README to the package:

```diff
--- a/manifest.verified.nuspec
+++ b/manifest.verified.nuspec
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>SamplePackage</id>
    <version>********</version>
    <authors>SamplePackage</authors>
+    <readme>README.md</readme>
    <description>Package Description</description>
    <repository type="git" commit="****************************************" />
    <dependencies>
      <group targetFramework="net8.0" />
    </dependencies>
  </metadata>
</package>

--- a/contents.verified.txt
+++ b/contents.verified.txt
/
+|-- README.md
|-- SamplePackage.nuspec
|-- lib
|   |-- net8.0
|   |   |-- SamplePackage.dll
```

Verifying package structure is part of an overall solution to prevent accidental package breaks. If you want to verify /
avoid breaking API changes, check out [Microsoft.CodeAnalysis.PublicApiAnalyzers](https://github.com/dotnet/roslyn-analyzers?tab=readme-ov-file#microsoftcodeanalysispublicapianalyzers).
If you want to follow packing best practices (validating a README, reproducible builds, etc.) check out
[meziantou's blog post](https://www.meziantou.net/ensuring-best-practices-for-nuget-packages.htm).

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

#### Excluding files

By default, the following files are excluded from the directory listing baseline:
- `[Content_Types].xml`
- `.psmdcp`
- `_rels/.rels`

If you'd to customize the file exclusion list, use `VerifySettings.AddNupkgDiffSettings()`.

```csharp
VerifySettings settings = new();
settings.AddNupkgDiffSettings(settings =>
{
    settings.ExcludedFiles = [new Regex(@"\.psmdcp$"), new Regex(@"^\[Content_Types\].xml$")];
});
```

### Custom scrubbers

.nuspec files often contain sources of verification churn. Use `VerifierSettings.ScrubNuspec()` like this:

```csharp
VerifySettings settings = new();
settings.ScrubNuspec();
```

which itself is a convenience method for `ScrubNuspecVersion()` and `ScrubNuspecCommit()`. Feel free to use them
separately if you'd like to verify either of these values.

### Referencing / locating a package built in the same solution

Verify is ideally suited for writing integration / snapshot tests of NuGet package contents.
However, ensuring a project creates a fresh NuGet package and locating it for testing can be
fragile.

To simplify the process, consider using [GetPackFromProject](https://github.com/MattKotsenas/GetPackFromProject)
to build the NuGet package for your `<ProjectReference>`s and place them in the test's
output directory for easy locating.

## ASCII tree

The ASCII art tree is inspired by the `tree` command, but with a few modifications to reduce the amount of
"noise" that occurs in diffs when files are added / removed.

## Icon

[Package](https://thenounproject.com/icon/package-1599428/) designed by [sandra](https://thenounproject.com/meisandra0583/)
from [The Noun Project](https://thenounproject.com).

using FluentAssertions;

using Microsoft.Build.Utilities.ProjectCreation;

namespace Verify.Nupkg.Tasks.Tests;

[TestClass]
public class VerifyNupkgTaskTests
{
    [TestMethod]
    public void IsPackableFalse_DoesNothing_NoWarnings()
    {
        // Arrange & Act
        TaskTestResult result = TaskTestHelper.BuildProjectWithTask((project, tempDir) =>
        {
            project.Property("IsPackable", "false");
        });

        // Assert
        result.Success.Should().BeTrue("the build should succeed");
        result.Warnings.Should().BeEmpty("no warnings should be emitted for non-packable projects");
        result.Errors.Should().BeEmpty("no errors should be emitted for non-packable projects");
    }

    [TestMethod]
    public void PackableProject_NoBaseline_EmitsMismatchWarning()
    {
        // Arrange & Act - Build without any baseline files
        TaskTestResult result = TaskTestHelper.BuildProjectWithTask((project, tempDir) =>
        {
            project.Property("IsPackable", "true");
        });

        // Assert - Task runs and emits warning about missing/mismatched baseline
        result.Success.Should().BeTrue("the build should succeed (warnings don't fail the build)");
        result.Warnings.Should().Contain(w => w.Code == "VNS1001", "should emit VNS1001 baseline mismatch warning when no baseline exists");
    }

    [TestMethod]
    public void PackableProject_WithMismatchedBaseline_EmitsWarning()
    {
        // Arrange - Create baseline with different content to trigger mismatch
        string mismatchedNuspec = @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
  <metadata>
    <id>DifferentPackage</id>
    <version>2.0.0</version>
  </metadata>
</package>";

        string mismatchedContents = @"/
|-- DifferentFile.dll";

        // Act
        TaskTestResult result = TaskTestHelper.BuildProjectWithTask(
            (project, tempDir) =>
            {
                project.Property("IsPackable", "true");
            },
            createBaseline: true,
            baselineNuspecContent: mismatchedNuspec,
            baselineContentsContent: mismatchedContents);

        // Assert
        result.Success.Should().BeTrue("the build should still succeed (warnings don't fail the build)");
        result.Warnings.Should().Contain(w => w.Code == "VNS1001", "should emit VNS1001 baseline mismatch warning");
    }

    [TestMethod]
    public void PackableProject_WithSymbols_EmitsWarningsForBothPackages()
    {
        // Arrange - Create mismatched baselines for both nupkg and snupkg
        string mismatchedNuspec = @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
  <metadata>
    <id>DifferentPackage</id>
  </metadata>
</package>";

        string mismatchedContents = @"/
|-- Wrong.dll";

        string customBaselineDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "Baselines");

        try
        {
            // Create baseline files for both packages
            Directory.CreateDirectory(customBaselineDir);
            File.WriteAllText(Path.Combine(customBaselineDir, "TestPackage.1.0.0.nupkg.manifest.verified.nuspec"), mismatchedNuspec);
            File.WriteAllText(Path.Combine(customBaselineDir, "TestPackage.1.0.0.nupkg.contents.verified.txt"), mismatchedContents);
            File.WriteAllText(Path.Combine(customBaselineDir, "TestPackage.1.0.0.snupkg.manifest.verified.nuspec"), mismatchedNuspec);
            File.WriteAllText(Path.Combine(customBaselineDir, "TestPackage.1.0.0.snupkg.contents.verified.txt"), mismatchedContents);

            // Act
            TaskTestResult result = TaskTestHelper.BuildProjectWithTask(
                (project, tempDir) =>
                {
                    project
                        .Property("IsPackable", "true")
                        .Property("IncludeSymbols", "true")
                        .Property("SymbolPackageFormat", "snupkg")
                        .Property("VerifyNupkgDirectory", customBaselineDir);
                },
                baselineDirectory: customBaselineDir);

            // Assert
            result.Success.Should().BeTrue("the build should succeed (warnings don't fail)");

            // Should have warnings for both packages
            var mismatchWarnings = result.Warnings.Where(w => w.Code == "VNS1001").ToList();
            mismatchWarnings.Should().HaveCount(2, "should emit VNS1001 for both nupkg and snupkg");
        }
        finally
        {
            if (Directory.Exists(Path.GetDirectoryName(customBaselineDir)))
            {
                Directory.Delete(Path.GetDirectoryName(customBaselineDir)!, recursive: true);
            }
        }
    }

    [TestMethod]
    public void PackableProject_TreatWarningsAsErrors_ElevatesWarningsToErrors()
    {
        // Arrange - Create baseline with different content to trigger mismatch
        string mismatchedNuspec = @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
  <metadata>
    <id>DifferentPackage</id>
    <version>2.0.0</version>
  </metadata>
</package>";

        string mismatchedContents = @"/
|-- DifferentFile.dll";

        // Act
        TaskTestResult result = TaskTestHelper.BuildProjectWithTask(
            (project, tempDir) =>
            {
                project
                    .Property("IsPackable", "true")
                    .Property("MSBuildTreatWarningsAsErrors", "true");
            },
            createBaseline: true,
            baselineNuspecContent: mismatchedNuspec,
            baselineContentsContent: mismatchedContents);

        // Assert
        result.Success.Should().BeFalse("the build should fail when TreatWarningsAsErrors is enabled");
        result.Errors.Should().Contain(e => e.Code == "VNS1001", "should emit VNS1001 as an error");
    }

    [TestMethod]
    public void PackableProject_Disabled_SkipsVerification()
    {
        // Arrange & Act
        TaskTestResult result = TaskTestHelper.BuildProjectWithTask((project, tempDir) =>
        {
            project
                .Property("IsPackable", "true")
                .Property("VerifyNupkgEnabled", "false");
        });

        // Assert
        result.Success.Should().BeTrue("the build should succeed");
        result.Warnings.Should().BeEmpty("no warnings should be emitted when verification is disabled");
        result.Errors.Should().BeEmpty("no errors should be emitted when verification is disabled");

        // Baseline directory should NOT be created when disabled
        result.BaselineDirectoryExists.Should().BeFalse("baseline directory should not be created when disabled");
    }

    [TestMethod]
    public void PackableProject_CustomBaselineDirectory_UsesSpecifiedPath()
    {
        // Arrange - Use a custom baseline directory with mismatched content
        // to verify that the custom directory is actually being used
        string customBaselineDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "CustomBaselines");

        string mismatchedNuspec = @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"">
  <metadata>
    <id>WrongPackageInCustomDir</id>
  </metadata>
</package>";

        string mismatchedContents = @"/
|-- CustomDirFile.dll";

        try
        {
            Directory.CreateDirectory(customBaselineDir);
            File.WriteAllText(Path.Combine(customBaselineDir, "TestPackage.1.0.0.nupkg.manifest.verified.nuspec"), mismatchedNuspec);
            File.WriteAllText(Path.Combine(customBaselineDir, "TestPackage.1.0.0.nupkg.contents.verified.txt"), mismatchedContents);

            // Act
            TaskTestResult result = TaskTestHelper.BuildProjectWithTask(
                (project, tempDir) =>
                {
                    project
                        .Property("IsPackable", "true")
                        .Property("VerifyNupkgDirectory", customBaselineDir);
                },
                baselineDirectory: customBaselineDir);

            // Assert - The task should use the custom directory and find the mismatched baseline
            result.Success.Should().BeTrue("the build should succeed (warnings don't fail)");
            result.Warnings.Should().Contain(w => w.Code == "VNS1001", 
                "should emit VNS1001 warning proving the custom baseline directory was used");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(Path.GetDirectoryName(customBaselineDir)))
            {
                Directory.Delete(Path.GetDirectoryName(customBaselineDir)!, recursive: true);
            }
        }
    }
}

using System.IO.Abstractions;
using System.Reflection;

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities.ProjectCreation;

namespace Verify.Nupkg.Tasks.Tests;

/// <summary>
/// Helper class to build test projects that use the VerifyNupkg task and capture the build results.
/// </summary>
internal static class TaskTestHelper
{
    private static readonly IFileSystem FileSystem = new FileSystem();

    /// <summary>
    /// Gets the path to the Verify.Nupkg.Tasks package output directory.
    /// </summary>
    public static string GetTasksPackagePath()
    {
        // The task assembly is in the same output directory as the test assembly
        string testAssemblyLocation = Assembly.GetExecutingAssembly().Location;
        string testOutputDir = Path.GetDirectoryName(testAssemblyLocation)!;

        // Navigate to the tasks assembly
        return Path.Combine(testOutputDir, "Verify.Nupkg.Tasks.dll");
    }

    /// <summary>
    /// Creates a project that references the VerifyNupkg task and builds it.
    /// </summary>
    /// <param name="configure">Action to configure the project before building.</param>
    /// <param name="baselineDirectory">Optional external baseline directory. If specified, this directory persists after the test.</param>
    /// <param name="createBaseline">If true, creates baseline files before building.</param>
    /// <param name="baselineNuspecContent">Content for the baseline nuspec file.</param>
    /// <param name="baselineContentsContent">Content for the baseline contents file.</param>
    /// <returns>A result object containing build success status, warnings, errors, and baseline file information.</returns>
    public static TaskTestResult BuildProjectWithTask(
        Action<ProjectCreator, IDirectoryInfo> configure,
        string? baselineDirectory = null,
        bool createBaseline = false,
        string? baselineNuspecContent = null,
        string? baselineContentsContent = null)
    {
        string taskPath = GetTasksPackagePath();

        using (FileSystem.CreateDisposableDirectory(
            RetryableTempDirectory.GetRandomTempPath(),
            dirInfo => new RetryableTempDirectory(dirInfo),
            out IDirectoryInfo tempDir))
        {
            // Use external baseline directory if provided, otherwise use temp directory
            bool useExternalBaselineDir = baselineDirectory != null;
            string effectiveBaselineDir = baselineDirectory ?? Path.Combine(tempDir.FullName, "NupkgBaselines");

            // Write the props and targets files to the temp directory
            string propsPath = Path.Combine(tempDir.FullName, "Verify.Nupkg.Tasks.props");
            string targetsPath = Path.Combine(tempDir.FullName, "Verify.Nupkg.Tasks.targets");

            File.WriteAllText(propsPath, CreateTaskPropsContent(taskPath, effectiveBaselineDir));
            File.WriteAllText(targetsPath, CreateTaskTargetsContent());

            using (PackageRepository.Create(tempDir.FullName, feeds: new Uri("https://api.nuget.org/v3/index.json")))
            {
                // Create a basic SDK-style project with task integration
                ProjectCreator project = ProjectCreator.Templates.SdkCsproj()
                    .Property("TargetFramework", "net8.0")
                    .Property("PackageId", "TestPackage")
                    .Property("PackageVersion", "1.0.0")
                    .Property("Authors", "Test")
                    .Property("Description", "Test package")
                    .Property("RepositoryType", "git")
                    .Property("RepositoryUrl", "https://github.com/test/test")
                    .Property("RepositoryCommit", "0000000000000000000000000000000000000000")
                    // Import the task props file
                    .Import(propsPath);

                // Allow the test to configure the project
                configure(project, tempDir);

                // Import the targets after configuration so the test can override properties
                project.Import(targetsPath);

                string projectPath = Path.Combine(tempDir.FullName, "TestProject.csproj");
                project.Save(projectPath);

                // Create baseline files if requested
                if (createBaseline && baselineNuspecContent != null && baselineContentsContent != null)
                {
                    Directory.CreateDirectory(effectiveBaselineDir);
                    string packageName = "TestPackage.1.0.0.nupkg";
                    File.WriteAllText(Path.Combine(effectiveBaselineDir, $"{packageName}.manifest.verified.nuspec"), baselineNuspecContent);
                    File.WriteAllText(Path.Combine(effectiveBaselineDir, $"{packageName}.contents.verified.txt"), baselineContentsContent);
                }

                // Build the project with Pack target
                project.TryBuild(restore: true, target: "Pack", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? _);

                // Capture baseline file information before the temp directory is cleaned up
                bool baselineDirectoryExists = Directory.Exists(effectiveBaselineDir);
                string[] baselineFiles = baselineDirectoryExists
                    ? Directory.GetFiles(effectiveBaselineDir, "*.verified.*")
                    : [];

                return new TaskTestResult(
                    Success: result,
                    Warnings: buildOutput.WarningEvents.ToList(),
                    Errors: buildOutput.ErrorEvents.ToList(),
                    Messages: buildOutput.MessageEvents.Where(m => m.Importance == MessageImportance.Normal || m.Importance == MessageImportance.High).ToList(),
                    BaselineDirectory: effectiveBaselineDir,
                    BaselineDirectoryExists: baselineDirectoryExists,
                    BaselineFiles: baselineFiles.Select(Path.GetFileName).ToArray()!);
            }
        }
    }

    // TODO: Use the real props and targets

    /// <summary>
    /// Creates MSBuild props content that sets up the task.
    /// </summary>
    private static string CreateTaskPropsContent(string taskPath, string baselineDirectory)
    {
        return $@"<Project>
  <PropertyGroup>
    <VerifyNupkgEnabled Condition="" '$(VerifyNupkgEnabled)' == '' "">true</VerifyNupkgEnabled>
    <VerifyNupkgDirectory Condition="" '$(VerifyNupkgDirectory)' == '' "">{baselineDirectory}</VerifyNupkgDirectory>
  </PropertyGroup>
  <UsingTask TaskName=""Verify.Nupkg.Tasks.VerifyNupkgTask"" AssemblyFile=""{taskPath}"" />
</Project>";
    }

    /// <summary>
    /// Creates MSBuild targets content that runs the task after Pack.
    /// </summary>
    private static string CreateTaskTargetsContent()
    {
        return @"<Project>
  <Target
    Name=""_VerifyNupkgAfterPack""
    AfterTargets=""Pack""
    DependsOnTargets=""Pack""
    Condition="" '$(VerifyNupkgEnabled)' != 'false' AND '$(IsPackable)' == 'true' "">

    <ItemGroup>
      <_VerifyNupkgPackageFiles Include=""@(NuGetPackOutput)"" Condition="" '%(Extension)' == '.nupkg' Or '%(Extension)' == '.snupkg' "" />
    </ItemGroup>

    <Verify.Nupkg.Tasks.VerifyNupkgTask
      PackageFiles=""@(_VerifyNupkgPackageFiles)""
      VerifyDirectory=""$(VerifyNupkgDirectory)"" />
  </Target>
</Project>";
    }
}

/// <summary>
/// Result of building a test project with the VerifyNupkg task.
/// </summary>
internal record TaskTestResult(
    bool Success,
    IReadOnlyList<BuildWarningEventArgs> Warnings,
    IReadOnlyList<BuildErrorEventArgs> Errors,
    IReadOnlyList<BuildMessageEventArgs> Messages,
    string BaselineDirectory,
    bool BaselineDirectoryExists,
    string[] BaselineFiles);

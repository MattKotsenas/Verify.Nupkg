using Microsoft.Build.Execution;
using Microsoft.Build.Utilities.ProjectCreation;
using System.IO.Abstractions;

namespace Verify.Nupkg.Tests;

/// <summary>
/// A class that automates the creation of a package for testing purposes.
///
/// The class builds the package in a temp directory to avoid any interactions with the main project, and injects
/// a task to copy the output package to the specified working directory. Tests are then expected to pick up
/// the package from that working directory.
/// </summary>
/// <remarks>
/// </remarks>
internal abstract class PackageCreator
{
    private readonly IFileSystem _fs = new FileSystem();

    /// <summary>
    /// Gets the name of the package. This is used as both the name of the .csproj file and the .nupkg file.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Creates the project file for the package.
    /// </summary>
    /// <param name="workingDirectory">
    /// The working directory where the package will be created. Use this if you need to create files that will be referenced
    /// from the project (e.g. content files).
    /// </param>
    /// <returns>
    /// A project definition that is ready to be saved and built.
    /// </returns>
    protected abstract ProjectCreator CreateCore(IDirectoryInfo workingDirectory);

    /// <summary>
    /// Creates the package in a temp directory and copies it to the specified destination directory.
    /// </summary>
    /// <param name="destinationDirectory">The directory to copy the .nupkg to.</param>
    /// <returns>A <see cref="IFileInfo"/> representing the built .nupkg file.</returns>
    /// <exception cref="Exception">If the build fails.</exception>
    public IFileInfo Create(IDirectoryInfo destinationDirectory)
    {
        IFileInfo destinationPackage = _fs.FileInfo.New(_fs.Path.Combine(destinationDirectory.FullName, $"{Name}.nupkg"));

        using (_fs.CreateDisposableDirectory(out IDirectoryInfo temp))
        {
            using (PackageRepository.Create(temp.FullName, feeds: new Uri("https://api.nuget.org/v3/index.json")))
            {
                CreateCore(temp)
                    .Target(name: "CopyPackageForTests", afterTargets: "Pack")
                        .Task(name: "Copy", parameters: new Dictionary<string, string?>
                        {
                            { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                            { "DestinationFiles", destinationPackage.FullName },
                        })
                    .Save(_fs.Path.Combine(temp.FullName, $"{Name}.csproj"))
                    .TryBuild(restore: true, target: "Pack", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

                if (!result)
                {
                    throw new Exception($"Failed to build in path '{temp.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
                }
            }
        }

        return destinationPackage;
    }
}

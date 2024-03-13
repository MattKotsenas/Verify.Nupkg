using FluentAssertions;
using Microsoft.Build.Execution;
using Microsoft.Build.Utilities.ProjectCreation;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSBuildTasks.Tests
{
    // TODO: AddPackageAsOutput = Off; GeneratePackageOnBuild = Off -- test pass
    // TODO: AddAsPackageOutput = On; GeneratePackageOnBuild = Off -- test fail



    public class Tests
    {
        private readonly IFileSystem _fs = new FileSystem();

        private IDirectoryInfo GetWorkingDirectory()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var dirInfo = _fs.FileInfo.New(assembly.Location).Directory ?? throw new Exception($"Unable to get directory from assembly location '{assembly.Location}'.");

            return dirInfo;
        }

        [Fact]
        public void Warning()
        {
            IDirectoryInfo workingDirectory = GetWorkingDirectory();

            //string verifyPackage = Directory.GetFiles(workingDirectory.FullName, "Verify.Nupkg.*.nupkg").Single();
            //new SimplePackage().Create(workingDirectory);

            //IFileInfo destinationPackage = _fs.FileInfo.New(_fs.Path.Combine(destinationDirectory.FullName, $"{Name}.nupkg"));

            using (_fs.CreateDisposableDirectory(dirInfo => new RetryableTempDirectory(dirInfo), out IDirectoryInfo temp))
            {
                using (PackageRepository.Create(temp.FullName))
                {
                    ProjectCreator leafProject = ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0").Save(_fs.Path.Combine(temp.FullName, "Leaf.csproj"));

                    ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0")
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.props"))
                        .ItemProjectReference(leafProject, metadata: new Dictionary<string, string?>
                        {
                            { "AddPackageAsOutput", "true" }
                        })
                        //.ItemPackageReference(package)
                        //CreateCore(temp)
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.targets"))
                        //.UsingTaskAssemblyFile("ValidateGeneratePackageOnBuild", Path.Combine(destinationDirectory.FullName, "")
                        //.Target(name: "CopyPackageForTests", afterTargets: "Pack")
                        //    .Task(name: "Copy", parameters: new Dictionary<string, string?>
                        //    {
                        //        { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                        //        { "DestinationFiles", destinationPackage.FullName },
                        //    })
                        .Save(_fs.Path.Combine(temp.FullName, $"Sample.csproj"))
                        .TryBuild(restore: true, target: "Build", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

                    buildOutput.Errors.Should().BeEmpty();
                    result.Should().BeTrue();

                    buildOutput.WarningEvents.Should().HaveCount(1).And.Subject.Single().Code.Should().Be("VP001");

                    // TODO: Test pass -- validate property metadata

                    //if (!result)
                    //{
                    //    throw new Exception($"Failed to build in path '{temp.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
                    //}
                }
            }
        }

        [Fact]
        public void Error()
        {
            IDirectoryInfo workingDirectory = GetWorkingDirectory();

            //string verifyPackage = Directory.GetFiles(workingDirectory.FullName, "Verify.Nupkg.*.nupkg").Single();
            //new SimplePackage().Create(workingDirectory);

            //IFileInfo destinationPackage = _fs.FileInfo.New(_fs.Path.Combine(destinationDirectory.FullName, $"{Name}.nupkg"));

            using (_fs.CreateDisposableDirectory(dirInfo => new RetryableTempDirectory(dirInfo), out IDirectoryInfo temp))
            {
                using (PackageRepository.Create(temp.FullName))
                {
                    ProjectCreator leafProject = ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0").Save(_fs.Path.Combine(temp.FullName, "Leaf.csproj"));

                    ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0")
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.props"))
                        .Property("MSBuildTreatWarningsAsErrors", "true")
                        .ItemProjectReference(leafProject, metadata: new Dictionary<string, string?>
                        {
                            { "AddPackageAsOutput", "true" }
                        })
                        //.ItemPackageReference(package)
                        //CreateCore(temp)
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.targets"))
                        //.UsingTaskAssemblyFile("ValidateGeneratePackageOnBuild", Path.Combine(destinationDirectory.FullName, "")
                        //.Target(name: "CopyPackageForTests", afterTargets: "Pack")
                        //    .Task(name: "Copy", parameters: new Dictionary<string, string?>
                        //    {
                        //        { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                        //        { "DestinationFiles", destinationPackage.FullName },
                        //    })
                        .Save(_fs.Path.Combine(temp.FullName, $"Sample.csproj"))
                        .TryBuild(restore: true, target: "Build", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

                    buildOutput.ErrorEvents.Should().HaveCount(1).And.Subject.Single().Code.Should().Be("VP001");

                    result.Should().BeFalse();


                    // TODO: Test pass -- validate property metadata

                    //if (!result)
                    //{
                    //    throw new Exception($"Failed to build in path '{temp.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
                    //}
                }
            }
        }

        [Fact]
        public void Off()
        {
            IDirectoryInfo workingDirectory = GetWorkingDirectory();

            //string verifyPackage = Directory.GetFiles(workingDirectory.FullName, "Verify.Nupkg.*.nupkg").Single();
            //new SimplePackage().Create(workingDirectory);

            //IFileInfo destinationPackage = _fs.FileInfo.New(_fs.Path.Combine(destinationDirectory.FullName, $"{Name}.nupkg"));

            using (_fs.CreateDisposableDirectory(dirInfo => new RetryableTempDirectory(dirInfo), out IDirectoryInfo temp))
            {
                using (PackageRepository.Create(temp.FullName))
                {
                    ProjectCreator leafProject = ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0").Save(_fs.Path.Combine(temp.FullName, "Leaf.csproj"));

                    ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0")
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.props"))
                        .ItemProjectReference(leafProject)
                        //.ItemPackageReference(package)
                        //CreateCore(temp)
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.targets"))
                        //.UsingTaskAssemblyFile("ValidateGeneratePackageOnBuild", Path.Combine(destinationDirectory.FullName, "")
                        //.Target(name: "CopyPackageForTests", afterTargets: "Pack")
                        //    .Task(name: "Copy", parameters: new Dictionary<string, string?>
                        //    {
                        //        { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                        //        { "DestinationFiles", destinationPackage.FullName },
                        //    })
                        .Save(_fs.Path.Combine(temp.FullName, $"Sample.csproj"))
                        .TryBuild(restore: true, target: "Build", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

                    buildOutput.ErrorEvents.Should().BeEmpty();
                    buildOutput.WarningEvents.Should().BeEmpty();
                    //.HaveCount(1).And.Subject.Single().Code.Should().Be("VP001");

                    result.Should().BeTrue();


                    // TODO: Test pass -- validate property metadata

                    //if (!result)
                    //{
                    //    throw new Exception($"Failed to build in path '{temp.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
                    //}
                }
            }
        }

        [Fact]
        public void Pass()
        {
            IDirectoryInfo workingDirectory = GetWorkingDirectory();

            //string verifyPackage = Directory.GetFiles(workingDirectory.FullName, "Verify.Nupkg.*.nupkg").Single();
            //new SimplePackage().Create(workingDirectory);

            //IFileInfo destinationPackage = _fs.FileInfo.New(_fs.Path.Combine(destinationDirectory.FullName, $"{Name}.nupkg"));

            string fromProjectReferenceMetadata = "Content with 'FromProjectReference' metadata:";
            string projectMetadata = "Content has 'Project' metadata:";

            using (_fs.CreateDisposableDirectory(dirInfo => new RetryableTempDirectory(dirInfo), out IDirectoryInfo temp))
            {
                using (PackageRepository.Create(temp.FullName))
                {
                    ProjectCreator leafProject = ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0").Property("GeneratePackageOnBuild", "true").Save(_fs.Path.Combine(temp.FullName, "Leaf.csproj"));

                    ProjectCreator.Templates.SdkCsproj(targetFramework: "net8.0")
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.props"))
                        .ItemProjectReference(leafProject, metadata: new Dictionary<string, string?>
                        {
                            { "AddPackageAsOutput", "true" }
                        })
                        //.ItemPackageReference(package)
                        //CreateCore(temp)
                        .Import(Path.Combine(workingDirectory.FullName, "build", "MSBuildTasks.targets"))
                        //.UsingTaskAssemblyFile("ValidateGeneratePackageOnBuild", Path.Combine(destinationDirectory.FullName, "")
                        //.Target(name: "CopyPackageForTests", afterTargets: "Pack")
                        //    .Task(name: "Copy", parameters: new Dictionary<string, string?>
                        //    {
                        //        { "SourceFiles", @"$(OutputPath)..\$(PackageId).$(PackageVersion).nupkg" },
                        //        { "DestinationFiles", destinationPackage.FullName },
                        //    })
                        .Target("OutputStuff", afterTargets: "Build")
                            .Task(name: "Message", parameters: new Dictionary<string, string?>
                            {
                                { "Text", $"{fromProjectReferenceMetadata}@(Content->WithMetadataValue('IsPackageFromProjectReference', 'true'))" },
                                { "Importance", "High" }
                            })
                            .Task(name: "Message", parameters: new Dictionary<string, string?>
                            {
                                { "Text", $"{projectMetadata}@(Content->HasMetadata('Project'))" },
                                { "Importance", "High" }
                            })
                        //"<Message Text=\"Dependent TargetOutputs is '@(_DependentPackageOutputs)'\" Importance=\"Low\" />")
                        .Save(_fs.Path.Combine(temp.FullName, $"Sample.csproj"))
                        .TryBuild(restore: true, target: "Build", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? outputs);

                    buildOutput.ErrorEvents.Should().BeEmpty();
                    buildOutput.WarningEvents.Should().BeEmpty();
                    //.HaveCount(1).And.Subject.Single().Code.Should().Be("VP001");

                    buildOutput.Messages.Should().ContainSingle(message => message.StartsWith(fromProjectReferenceMetadata));
                    buildOutput.Messages.Should().ContainSingle(message => message.StartsWith(projectMetadata));

                    result.Should().BeTrue();


                    // TODO: Test pass -- validate property metadata

                    //if (!result)
                    //{
                    //    throw new Exception($"Failed to build in path '{temp.FullName}'. Errors: {string.Join(Environment.NewLine, buildOutput.Errors)}");
                    //}
                }
            }
        }
    }
}

namespace Verify.Nupkg.Tests;

[TestClass]
public partial class NuspecScrubbingTests
{
    [TestMethod]
    public Task DoNotScrubGitExtensionOnRepoUrl()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>PackageWithRepoGitExtension</id>
                <version>1.0.0</version>
                <authors>PackageWithRepoGitExtension</authors>
                <description>Package Description</description>
                <repository type="git" url="https://github.com/MattKotsenas/Verify.Nupkg.git" commit="0e4d1b598f350b3dc675018d539114d1328189ef" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task AddGitExtensionToRepoUrl()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>SimplePackageWithSymbols</id>
                <version>1.0.0</version>
                <authors>SimplePackageWithSymbols</authors>
                <readme>README.md</readme>
                <description>Package Description</description>
                <repository type="git" url="https://github.com/MattKotsenas/Verify.Nupkg" branch="dev" commit="0e4d1b598f350b3dc675018d539114d1328189ef" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task DoNotScrubNonHttpsRepoUrl()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>PackageWithoutRepoHttps</id>
                <version>1.0.0</version>
                <authors>PackageWithoutRepoHttps</authors>
                <description>Package Description</description>
                <repository type="git" url="http://github.com/MattKotsenas/Verify.Nupkg" commit="0e4d1b598f350b3dc675018d539114d1328189ef" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task DoNotScrubNonGitHubDomainRepoUrl()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>PackageWithoutRepoGitHubDomain</id>
                <version>1.0.0</version>
                <authors>PackageWithoutRepoGitHubDomain</authors>
                <description>Package Description</description>
                <repository type="git" url="https://bitbucket.com/my/cool/project" commit="0e4d1b598f350b3dc675018d539114d1328189ef" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoCommit()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>PackageWithoutRepoUrlOrCommitOrBranch</id>
                <version>1.0.0</version>
                <authors>PackageWithoutRepoUrlOrCommitOrBranch</authors>
                <description>Package Description</description>
                <repository type="git" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoUrl()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>PackageWithoutRepoUrlOrCommitOrBranch</id>
                <version>1.0.0</version>
                <authors>PackageWithoutRepoUrlOrCommitOrBranch</authors>
                <description>Package Description</description>
                <repository type="git" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoBranch()
    {
        string manifest =
            """
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>PackageWithoutRepoUrlOrCommitOrBranch</id>
                <version>1.0.0</version>
                <authors>PackageWithoutRepoUrlOrCommitOrBranch</authors>
                <description>Package Description</description>
                <repository type="git" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest")).ScrubNuspec();
    }

    [TestMethod]
    public Task OnlyOptInScrubbersRun()
    {
        string manifest =
            """
            <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
              <metadata>
                <id>SimplePackageWithSymbols</id>
                <version>1.0.0</version>
                <authors>SimplePackageWithSymbols</authors>
                <readme>README.md</readme>
                <description>Package Description</description>
                <repository type="git" url="https://github.com/MattKotsenas/Verify.Nupkg.git" branch="dev" commit="0e4d1b598f350b3dc675018d539114d1328189ef" />
                <dependencies>
                  <group targetFramework="net8.0" />
                </dependencies>
              </metadata>
            </package>
            """;

        // In this test we intentionally _do not_ include these scrubbers:
        //  - Version
        //  - Schema
        // to validate that only scrubbers we opt-in to are applied.
        return Verifier.Verify(new Target(extension: "nuspec", data: manifest, name: "manifest"))
            .ScrubNuspecCommit()
            .ScrubNuspecRepositoryUrl()
            .ScrubNuspecBranch();
    }
}

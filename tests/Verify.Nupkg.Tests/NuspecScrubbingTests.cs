namespace Verify.Nupkg.Tests;

public class NuspecScrubbingTests
{
    private string _packageWithRepoGitExtension = SamplePackages.Instance.PackageWithRepoGitExtension.Value.FullName;
    private string _packageWithoutRepoGitExtension = SamplePackages.Instance.PackageWithoutRepoGitExtension.Value.FullName;
    private string _packageWithoutRepoHttps = SamplePackages.Instance.PackageWithoutRepoHttps.Value.FullName;
    private string _packageWithoutRepoGitHubDomain = SamplePackages.Instance.PackageWithoutRepoGitHubDomain.Value.FullName;
    private string _packageWithoutRepoUrl = SamplePackages.Instance.PackageWithoutRepoUrl.Value.FullName;
    private string _packageWithoutRepoCommit = SamplePackages.Instance.PackageWithoutRepoCommit.Value.FullName;

    [Fact]
    public Task DoNotScrubGitExtensionOnRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithRepoGitExtension, settings);
    }

    [Fact]
    public Task AddGitExtensionToRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoGitExtension, settings);
    }

    [Fact]
    public Task DoNotScrubNonHttpsRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoHttps, settings);
    }

    [Fact]
    public Task DoNotScrubNonGitHubDomainRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoGitHubDomain, settings);
    }

    [Fact]
    public Task SkipScrubbingForRepoWithNoCommit()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoCommit, settings);
    }

    [Fact]
    public Task SkipScrubbingForRepoWithNoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoUrl, settings);
    }
}

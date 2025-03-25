namespace Verify.Nupkg.Tests;

[TestClass]
public partial class NuspecScrubbingTests
{
    private readonly string _packageWithRepoGitExtension = SamplePackages.Instance.PackageWithRepoGitExtension.Value.FullName;
    private readonly string _packageWithoutRepoGitExtension = SamplePackages.Instance.PackageWithoutRepoGitExtension.Value.FullName;
    private readonly string _packageWithoutRepoHttps = SamplePackages.Instance.PackageWithoutRepoHttps.Value.FullName;
    private readonly string _packageWithoutRepoGitHubDomain = SamplePackages.Instance.PackageWithoutRepoGitHubDomain.Value.FullName;
    private readonly string _packageWithoutRepoUrl = SamplePackages.Instance.PackageWithoutRepoUrl.Value.FullName;
    private readonly string _packageWithoutRepoCommit = SamplePackages.Instance.PackageWithoutRepoCommit.Value.FullName;
    private readonly string _packageWithoutRepoBranch = SamplePackages.Instance.PackageWithoutRepoBranch.Value.FullName;

    [TestMethod]
    public Task DoNotScrubGitExtensionOnRepoUrl()
    {
        return VerifyFile(_packageWithRepoGitExtension).ScrubNuspec();
    }

    [TestMethod]
    public Task AddGitExtensionToRepoUrl()
    {
        return VerifyFile(_packageWithoutRepoGitExtension).ScrubNuspec();
    }

    [TestMethod]
    public Task DoNotScrubNonHttpsRepoUrl()
    {
        return VerifyFile(_packageWithoutRepoHttps).ScrubNuspec();
    }

    [TestMethod]
    public Task DoNotScrubNonGitHubDomainRepoUrl()
    {
        return VerifyFile(_packageWithoutRepoGitHubDomain).ScrubNuspec();
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoCommit()
    {
        return VerifyFile(_packageWithoutRepoCommit).ScrubNuspec();
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoUrl()
    {
        return VerifyFile(_packageWithoutRepoUrl).ScrubNuspec();
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoBranch()
    {
        return VerifyFile(_packageWithoutRepoBranch).ScrubNuspec();
    }
}

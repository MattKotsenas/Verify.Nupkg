﻿namespace Verify.Nupkg.Tests;

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
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithRepoGitExtension, settings);
    }

    [TestMethod]
    public Task AddGitExtensionToRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoGitExtension, settings);
    }

    [TestMethod]
    public Task DoNotScrubNonHttpsRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoHttps, settings);
    }

    [TestMethod]
    public Task DoNotScrubNonGitHubDomainRepoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoGitHubDomain, settings);
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoCommit()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoCommit, settings);
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoUrl()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoUrl, settings);
    }

    [TestMethod]
    public Task SkipScrubbingForRepoWithNoBranch()
    {
        VerifySettings settings = new();
        settings.UseUniqueDirectory();
        settings.ScrubNuspec();

        return VerifyFile(_packageWithoutRepoBranch, settings);
    }
}

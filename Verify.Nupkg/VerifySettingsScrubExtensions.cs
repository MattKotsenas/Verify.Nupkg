namespace VerifyTests;

/// <summary>
/// Extension methods that scrub common changes from nupkg files.
/// </summary>
public static class VerifySettingsScrubExtensions
{
    /// <summary>
    /// Scrub the nuspec file of common changes.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="VerifySettings"/> to modify.
    /// </param>
    /// <remarks>
    /// Sources of common changes:
    /// - Package version
    /// - Commit hash
    /// </remarks>
    /// <returns>The <see cref="VerifySettings"/> for chaining.</returns>
    public static VerifySettings ScrubNuspec(this VerifySettings settings)
    {
        settings.ScrubNuspecVersion();
        settings.ScrubNuspecCommit();
        settings.ScrubNuspecRepositoryUrl();

        return settings;
    }

    /// <summary>
    /// Scrub the nuspec file of common changes.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SettingsTask"/> to modify.
    /// </param>
    /// <remarks>
    /// Sources of common changes:
    /// - Package version
    /// - Commit hash
    /// </remarks>
    /// <returns>The <see cref="SettingsTask"/> for chaining.</returns>
    public static SettingsTask ScrubNuspec(this SettingsTask settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.CurrentSettings.ScrubNuspec();
        return settings;
    }

    /// <summary>
    /// Scrub the version from the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="VerifierSettings"/> to modify.
    /// </param>
    /// <remarks>
    /// In some scenarios, package versions are frequent and obvious changes. In these cases, scrub the version
    /// to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="VerifySettings"/> for chaining.</returns>
    public static VerifySettings ScrubNuspecVersion(this VerifySettings settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.AddScrubber(extension: "nuspec", sb => new VersionScrubber().Scrub(sb));

        return settings;
    }

    /// <summary>
    /// Scrub the version from the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SettingsTask"/> to modify.
    /// </param>
    /// <remarks>
    /// In some scenarios, package versions are frequent and obvious changes. In these cases, scrub the version
    /// to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="SettingsTask"/> for chaining.</returns>
    public static SettingsTask ScrubNuspecVersion(this SettingsTask settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.CurrentSettings.ScrubNuspecVersion();
        return settings;
    }

    /// <summary>
    /// Scrub the commit info from the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="VerifierSettings"/> to modify.
    /// </param>
    /// <remarks>
    /// In some scenarios, the commit used in the package are frequent and obvious changes. In these cases, scrub the commit
    /// to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="VerifySettings"/> for chaining.</returns>
    public static VerifySettings ScrubNuspecCommit(this VerifySettings settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.AddScrubber(extension: "nuspec", sb => new CommitScrubber().Scrub(sb));

        return settings;
    }

    /// <summary>
    /// Scrub the commit info from the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SettingsTask"/> to modify.
    /// </param>
    /// <remarks>
    /// In some scenarios, the commit used in the package are frequent and obvious changes. In these cases, scrub the commit
    /// to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="SettingsTask"/> for chaining.</returns>
    public static SettingsTask ScrubNuspecCommit(this SettingsTask settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.CurrentSettings.ScrubNuspecCommit();
        return settings;
    }

    /// <summary>
    /// Scrub and normalize the repository URL in the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="VerifierSettings"/> to modify.
    /// </param>
    /// <remarks>
    /// GitHub repository URLs can be optionally have a `.git` suffix. This method normalizes the URL to always end with .git
    /// to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="VerifySettings"/> for chaining.</returns>
    public static VerifySettings ScrubNuspecRepositoryUrl(this VerifySettings settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.AddScrubber(extension: "nuspec", sb => new RepositoryUrlScrubber().Scrub(sb));

        return settings;
    }

    /// <summary>
    /// Scrub and normalize the repository URL in the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SettingsTask"/> to modify.
    /// </param>
    /// <remarks>
    /// GitHub repository URLs can be optionally have a `.git` suffix. This method normalizes the URL to always end with .git
    /// to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="SettingsTask"/> for chaining.</returns>
    public static SettingsTask ScrubNuspecRepositoryUrl(this SettingsTask settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.CurrentSettings.ScrubNuspecRepositoryUrl();
        return settings;
    }
}

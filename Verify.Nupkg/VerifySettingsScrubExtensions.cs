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
        settings.ScrubNuspecSchema();
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
    /// Scrub the XML schema from the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="VerifierSettings"/> to modify.
    /// </param>
    /// <remarks>
    /// The NuGet client will always use the oldest schema version it can. Additionally, a new schema version was released for
    /// SemVer version numbers (i.e. v2 and v3 here: https://github.com/NuGet/NuGet.Client/blob/8c972cdff5b1194d7c37384fca5816a33ffbe0c4/src/NuGet.Core/NuGet.Packaging/PackageCreation/Authoring/ManifestSchemaUtility.cs).
    ///
    /// The result of these two factors is that a version bump (such as from 1.0.0-deadbeef to 1.0.1) may result in a nuspec diff.
    /// Thus scrub the xmlns for the NuSpec schema to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="VerifySettings"/> for chaining.</returns>
    public static VerifySettings ScrubNuspecSchema(this VerifySettings settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.AddScrubber(extension: "nuspec", sb => new SchemaScrubber().Scrub(sb));

        return settings;
    }

    /// <summary>
    /// Scrub the XML schema from the nuspec file.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="SettingsTask"/> to modify.
    /// </param>
    /// <remarks>
    /// The NuGet client will always use the oldest schema version it can. Additionally, a new schema version was released for
    /// SemVer version numbers (i.e. v2 and v3 here: https://github.com/NuGet/NuGet.Client/blob/8c972cdff5b1194d7c37384fca5816a33ffbe0c4/src/NuGet.Core/NuGet.Packaging/PackageCreation/Authoring/ManifestSchemaUtility.cs).
    ///
    /// The result of these two factors is that a version bump (such as from 1.0.0-deadbeef to 1.0.1) may result in a nuspec diff.
    /// As a result, scrub the xmlns schema to reduce the noise in the diff.
    /// </remarks>
    /// <returns>The <see cref="SettingsTask"/> for chaining.</returns>
    public static SettingsTask ScrubNuspecSchema(this SettingsTask settings)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }

        settings.CurrentSettings.ScrubNuspecSchema();
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

        settings.AddScrubber(extension: "nuspec", sb => new RepositoryCommitScrubber().Scrub(sb));

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
    /// GitHub repository URLs can be optionally have a `.git` suffix. This method normalizes the URL to always end with .git.
    /// This method also normalizes GitHub urls to replace the project name so that forks do not cause diffs.
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

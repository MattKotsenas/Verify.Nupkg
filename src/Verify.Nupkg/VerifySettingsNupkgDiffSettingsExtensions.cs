namespace VerifyTests;

/// <summary>
/// Extension methods for <see cref="VerifySettings"/> to add <see cref="NupkgDiffSettings"/>.
/// </summary>
public static class VerifySettingsNupkgDiffSettingsExtensions
{
    /// <summary>
    /// Add custom nupkg diff settings.
    /// </summary>
    /// <param name="settings">The <see cref="VerifySettings"/> to configure.</param>
    /// <param name="action">The <see cref="NupkgDiffSettings"/> to configure.</param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="settings"/> or <paramref name="action"/> is <c>null</c>.
    /// </exception>
    /// <returns>The <see cref="VerifySettings"/> for chaining.</returns>
    public static VerifySettings AddNupkgDiffSettings(this VerifySettings settings, Action<NupkgDiffSettings> action)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
        if (action is null) { throw new ArgumentNullException(nameof(action)); }

        var nupkgDiffSettings = new NupkgDiffSettings();
        action(nupkgDiffSettings);
        settings.Context[NupkgDiffSettings.ContextKey] = nupkgDiffSettings;

        return settings;
    }

    /// <summary>
    /// Add custom nupkg settings
    /// </summary>
    /// <param name="settings">The <see cref="SettingsTask"/> to configure.</param>
    /// <param name="action">The <see cref="NupkgDiffSettings"/> to configure.</param>
    /// <returns>The <see cref="SettingsTask"/> for chaining.</returns>
    public static SettingsTask AddNupkgDiffSettings(this SettingsTask settings, Action<NupkgDiffSettings> action)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
        if (action is null) { throw new ArgumentNullException(nameof(action)); }

        settings.CurrentSettings.AddNupkgDiffSettings(action);
        return settings;
    }

    internal static NupkgDiffSettings GetNupkgDiffSettingsOrDefault(this IReadOnlyDictionary<string, object> dictionary)
    {
        if (dictionary.TryGetValue(NupkgDiffSettings.ContextKey, out object? value))
        {
            if (value is NupkgDiffSettings nds)
            {
                return nds;
            }
        }

        return new();
    }
}

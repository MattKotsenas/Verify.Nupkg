namespace Verify.Nupkg;

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
    public static void AddNupkgDiffSettings(this VerifySettings settings, Action<NupkgDiffSettings> action)
    {
        if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
        if (action is null) { throw new ArgumentNullException(nameof(action)); }

        var nupkgDiffSettings = new NupkgDiffSettings();
        action(nupkgDiffSettings);
        settings.Context[NupkgDiffSettings.ContextKey] = nupkgDiffSettings;
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

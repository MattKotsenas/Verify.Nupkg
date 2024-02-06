using System.IO.Compression;
using System.Runtime.CompilerServices;
using Verify.Nupkg;

[assembly: InternalsVisibleTo("Verify.Nupkg.Tests")]

namespace VerifyTests;

/// <summary>
/// Plugin for Verify to handle .nupkg files.
/// </summary>
public static class VerifyNupkg
{
    // NOTE: Do not change the name of the `Initialize` method or `Initialized` property
    // as it is used via reflection by VerifierSettings.InitializePlugins().

    /// <summary>
    /// Gets if the plugin has been initialized or not.
    /// </summary>
    public static bool Initialized { get; private set; }

    /// <summary>
    /// Register the .nupkg file converter for Verify.
    /// </summary>
    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();

        VerifierSettings.RegisterFileConverter(
            fromExtension: "nupkg",
            conversion: async (stream, settings) =>
            {
                NupkgDiffSettings diffSettings = settings.GetNupkgDiffSettingsOrDefault();

                using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

                using Stream nuspecStream = zip.Entries.Single(e => e.Name.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).Open();
                using StreamReader reader = new(nuspecStream);

                string manifest = await reader.ReadToEndAsync();
                string contents = zip.ListPackageContents(diffSettings.ExcludedFiles);

                Target[] targets = [
                    new Target(extension: "nuspec", data: manifest, name: "manifest"),
                    new Target(extension: "txt", data: contents, name: "contents"),
                ];

                return new ConversionResult(info: null, targets: targets);
            });
    }
}

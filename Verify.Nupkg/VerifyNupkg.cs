using System.IO.Compression;

namespace VerifyTests;

public static class VerifyNupkg
{
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
            conversion: (stream, settings) =>
            {
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

                using Stream nuspecStream = zip.Entries.Single(e => e.Name.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase)).Open();
                using StreamReader reader = new(nuspecStream);

                string manifest = reader.ReadToEnd();
                string contents = zip.ListPackageContents();

                Target[] targets = [
                    new Target(extension: "nuspec", data: manifest, name: "manifest"),
                    new Target(extension: "txt", data: contents, name: "contents"),
                ];

                return new ConversionResult(info: null, targets: targets);
            });
    }
}

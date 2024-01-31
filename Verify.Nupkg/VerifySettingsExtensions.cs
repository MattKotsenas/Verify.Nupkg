namespace Verify.Nupkg;

public static class VerifySettingsExtensions
{
    private static readonly NuspecScrubber _scrubber = new();

    public static void ScrubNuspec(this VerifySettings settings)
    {
        settings.ScrubLinesWithReplace(
            extension: "nuspec",
            line =>
        {
            line = _scrubber.ScrubCommit(line);
            line = _scrubber.ScrubVersion(line);

            return line;
        });
    }
}

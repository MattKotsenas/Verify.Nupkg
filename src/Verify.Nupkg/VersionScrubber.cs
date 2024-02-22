using System.Xml.Linq;

namespace VerifyTests;

internal class VersionScrubber : NuspecScrubberBase
{
    protected override void Scrub(XDocument document)
    {
        XElement[] versionElements = document.DescendantsAnyNS("version").ToArray();

        foreach (XElement versionElement in versionElements)
        {
            // Length chosen because it looks nice in diffs. Any number will work, but it should be consistent.
            string replacement = new('*', count: 8);

            versionElement.SetValue(replacement);
        }
    }
}

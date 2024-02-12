using System.Xml.Linq;

namespace VerifyTests;

internal interface INuspecScrubber
{
    void Scrub(XDocument document);
}

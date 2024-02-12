using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Linq;

namespace VerifyTests;

internal static class XmlExtensions
{
    public static bool TryParseForScrubbing(string text, [NotNullWhen(true)] out XDocument? document, LoadOptions options = LoadOptions.PreserveWhitespace)
    {
        try
        {
            document = XDocument.Parse(text, options);
            return true;
        }
        catch (XmlException)
        {
            document = null;
        }

        return false;
    }

    public static IEnumerable<XElement> DescendantsAnyNS<T>(this T source, string localName) where T : XContainer
    {
        return source.Descendants().Where(e => e.Name.LocalName == localName);
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Linq;

namespace Verify.Nupkg;

internal static class XElementExtensions
{
    public static bool TryParseForScrubbing(string text, [NotNullWhen(true)] out XElement? element, LoadOptions options = LoadOptions.PreserveWhitespace)
    {
        try
        {
            element = XElement.Parse(text, options);
            return true;
        }
        catch (XmlException)
        {
            element = null;
        }

        return false;
    }
}

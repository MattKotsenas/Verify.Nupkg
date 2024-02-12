using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace VerifyTests;

internal abstract class NuspecScrubberBase
{
    public void Scrub(StringBuilder builder)
    {
        string text = builder.ToString();

        if (XmlExtensions.TryParseForScrubbing(text, out XDocument? document))
        {
            Scrub(document);

            builder.Clear();

            using (XmlWriter writer = XmlWriter.Create(builder, new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
            }))
            {
                document.Save(writer);
            }

            builder.TrimStart();
        }
    }

    protected abstract void Scrub(XDocument document);
}

using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace VerifyTests;

/// <summary>
/// Outer "runner" class that is responsible for:
///
/// 1. Converting the nuspec file from <see cref="StringBuilder"/> to <see cref="XDocument"/>
/// 2. Running the scrubbers in the <see cref="VerifySettings.Context"/>
/// 3. Converting the XML back to a string
/// </summary>
/// <remarks>
/// It would be simpler to make this a base class and let each scrubber be responsible for the de/re-serialization,
/// however sharing a single XDocument among all the scrubbers is significantly faster.
/// </remarks>
internal class NuspecScrubberRunner
{
    private readonly VerifySettings _settings;

    public NuspecScrubberRunner(VerifySettings settings)
    {
        _settings = settings;
    }

    public void Scrub(StringBuilder builder)
    {
        if (!_settings.Context.TryGetValue(NuspecScrubbers.ContextKey, out var contextValue))
        {
            return;
        }
        if (contextValue is not NuspecScrubbers scrubbers)
        {
            return;
        }

        string text = builder.ToString();

        if (XmlExtensions.TryParseForScrubbing(text, out XDocument? document))
        {
            foreach (INuspecScrubber scrubber in scrubbers)
            {
                scrubber.Scrub(document);
            }

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
}

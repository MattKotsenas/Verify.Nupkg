using System.Xml.Linq;

namespace VerifyTests;

internal class RepositoryUrlScrubber : NuspecScrubberBase
{
    protected override void Scrub(XDocument document)
    {
        XElement[] repositoryElements = document.DescendantsAnyNS("repository").ToArray();

        foreach (XElement repositoryElement in repositoryElements)
        {
            XAttribute? urlAttribute = repositoryElement.Attribute("url");

            if (urlAttribute is null)
            {
                return;
            }

            UriBuilder url = new(urlAttribute.Value);

            // Exclude non-GitHub URLs, and other formats like SSH
            if (!(url.Host == "github.com" && url.Scheme == "https"))
            {
                return;
            }

            if (!url.Path.EndsWith(".git"))
            {
                url.Path += ".git";
                urlAttribute.SetValue(url.Uri.ToString());
            }
        }
    }
}

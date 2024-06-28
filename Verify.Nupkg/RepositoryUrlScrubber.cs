using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace VerifyTests;

internal class RepositoryUrlScrubber : NuspecScrubberBase
{
    private static readonly Regex ProjectRegex = new("^/([^/]*)");
    private static readonly string ProjectNameReplacement = "/********";

    protected override void Scrub(XDocument document)
    {
        XElement[] repositoryElements = document.DescendantsAnyNS("repository").ToArray();

        foreach (XElement repositoryElement in repositoryElements)
        {
            XAttribute? urlAttribute = repositoryElement.Attribute("url");

            if (urlAttribute is null)
            {
                continue;
            }

            UriBuilder url = new(urlAttribute.Value);

            // Exclude non-GitHub URLs, and other formats like SSH
            if (!(url.Host == "github.com" && url.Scheme == "https"))
            {
                continue;
            }

            if (!url.Path.EndsWith(".git"))
            {
                url.Path += ".git";
            }

            url.Path = ProjectRegex.Replace(url.Path, ProjectNameReplacement);

            urlAttribute.SetValue(url.Uri.ToString());
        }
    }
}

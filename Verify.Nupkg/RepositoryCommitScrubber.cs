using System.Xml.Linq;

namespace VerifyTests;

internal class RepositoryCommitScrubber : NuspecScrubberBase
{
    protected override void Scrub(XDocument document)
    {
        XElement[] repositoryElements = document.DescendantsAnyNS("repository").ToArray();

        foreach (XElement repositoryElement in repositoryElements)
        {
            XAttribute? commitAttribute = repositoryElement.Attribute("commit");

            if (commitAttribute is null)
            {
                return;
            }

            // Length chosen because it's the standard length of a git SHA. Any number will work, but it should be consistent.
            string replacement = new('*', count: 40);

            commitAttribute.SetValue(replacement);
        }
    }
}

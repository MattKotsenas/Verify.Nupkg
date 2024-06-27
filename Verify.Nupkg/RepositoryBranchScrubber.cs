using System.Xml.Linq;

namespace VerifyTests;

internal class RepositoryBranchScrubber : NuspecScrubberBase
{
    protected override void Scrub(XDocument document)
    {
        XElement[] repositoryElements = document.DescendantsAnyNS("repository").ToArray();

        foreach (XElement repositoryElement in repositoryElements)
        {
            XAttribute? commitAttribute = repositoryElement.Attribute("branch");

            if (commitAttribute is null)
            {
                continue;
            }

            // Length chosen only because it looks nice in diffs. Any number will work, but it should be consistent.
            string replacement = new('*', count: 8);

            commitAttribute.SetValue(replacement);
        }
    }
}

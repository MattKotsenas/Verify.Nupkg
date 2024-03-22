using System.Xml.Linq;

namespace VerifyTests;

internal class SchemaScrubber : NuspecScrubberBase
{
    protected override void Scrub(XDocument document)
    {
        if (document.Root is null)
        {
            return;
        }

        // Remove namespace declaration from the document
        document.Root.Attributes().Where(e => e.IsNamespaceDeclaration).Remove();

        // Strip the namespace from the elements that already have it as part of their DOM
        foreach (var node in document.Root.DescendantsAndSelf())
        {
            node.Name = node.Name.LocalName;
        }
    }
}

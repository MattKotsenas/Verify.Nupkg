using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace VerifyTests;

internal static class ZipArchiveExtensions
{
    public static string ListPackageContents(this ZipArchive zip, IEnumerable<Regex> excludedMatches)
    {
        PathNode root = new() { Name = "/" };

        foreach (ZipArchiveEntry entry in zip.Entries.OrderBy(e => e.FullName, StringComparer.Ordinal))
        {
            if (excludedMatches.Any(m => m.IsMatch(entry.FullName)))
            {
                continue;
            }

            string[] segments = entry.FullName.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            PathNode current = root;
            foreach (string segment in segments)
            {
                PathNode? target = current.Children.SingleOrDefault(c => c.Name == segment);
                if (target is null)
                {
                    PathNode newNode = new() { Name = segment };
                    current.Children.Add(newNode);

                    current = newNode;
                }
                else
                {
                    current = target;
                }
            }

        }

        // Spectre.Console requires the top of a tree to be a Tree and not a TreeNode, so start with a dummy TreeNode
        // and skip the root PathNode
        TreeNode tn = new(new Markup(""));
        Walk(tn, [.. root.Children]);

        // Create our Tree root and skip the dummy TreeNode from above
        Tree tree = new("/")
        {
            Guide = new DiffableTreeGuide()
        };
        tree.AddNodes(tn.Nodes);

        TestConsole console = new();
        console.Write(tree);

        return console.Output;
    }

    private static void Walk(TreeNode treeNode, params PathNode[] pathNodes)
    {
        foreach (PathNode pathNode in pathNodes)
        {
            TreeNode childTree = treeNode.AddNode(Markup.Escape(pathNode.Name));
            foreach (PathNode childPath in pathNode.Children.OrderBy(c => c.Name, StringComparer.Ordinal))
            {
                Walk(childTree, childPath);
            }
        }
    }

    private class DiffableTreeGuide : TreeGuide
    {
        public override string GetPart(TreeGuidePart part)
        {
            // Using `End` causes diff noise, so use `Fork` instead
            if (part == TreeGuidePart.End)
            {
                part = TreeGuidePart.Fork;
            }

            return Ascii.GetPart(part);
        }
    }
}

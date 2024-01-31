using Spectre.Console;
using Spectre.Console.Testing;
using System.IO.Compression;

namespace VerifyTests;

internal static class ZipArchiveExtensions
{
    public static string ListPackageContents(this ZipArchive zip)
    {
        PathNode root = new PathNode { Name = "/" };

        foreach (ZipArchiveEntry entry in zip.Entries.OrderBy(e => e.FullName))
        {
            // Skip the .psmdcp and [Content_Types].xml because they aren't influenced by the user
            if (!entry.Name.EndsWith(".psmdcp", StringComparison.OrdinalIgnoreCase) && !(entry.FullName == "[Content_Types].xml"))
            {
                string[] segments = entry.FullName.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                PathNode current = root;
                foreach (string segment in segments)
                {
                    PathNode? target = current.Children.SingleOrDefault(c => c.Name == segment);
                    if (target is null)
                    {
                        PathNode newNode = new PathNode { Name = segment };
                        current.Children.Add(newNode);

                        current = newNode;
                    }
                    else
                    {
                        current = target;
                    }
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
            Guide = TreeGuide.Ascii
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
            TreeNode childTree = treeNode.AddNode(pathNode.Name);
            foreach (PathNode childPath in pathNode.Children.OrderBy(c => c.Name))
            {
                Walk(childTree, childPath);
            }
        }
    }
}

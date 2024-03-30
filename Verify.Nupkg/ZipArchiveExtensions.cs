using System.IO.Compression;
using System.Text;
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

        return Walk(root);
    }

    private static string Walk(PathNode root)
    {
        StringBuilder sb = new();

        Walk(root, 0, sb);

        return sb.ToString();
    }

    private static void Walk(PathNode node, int depth, StringBuilder sb)
    {
        StringBuilder prefix = new();

        if (depth - 1 > 0)
        {
            foreach (var i in Enumerable.Range(0, depth - 1))
            {
                prefix.Append("|   ");
            }
        }

        if (depth > 0)
        {
            prefix.Append("|-- ");
        }

        sb.Append(prefix);
        sb.AppendLine(node.Name);

        foreach (PathNode child in node.Children.OrderBy(c => c.Name, StringComparer.Ordinal))
        {
            Walk(child, depth + 1, sb);
        }
    }
}

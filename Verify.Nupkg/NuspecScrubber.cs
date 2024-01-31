using System.Text.RegularExpressions;

namespace Verify.Nupkg;

internal class NuspecScrubber
{
    private static readonly Regex _commitRegex = new("commit=\"([^\"]*)\"");
    private static readonly Regex _versionRegex = new("<version>([^<]*)</version>");

    public string ScrubCommit(string line)
    {
        if (!line.TrimStart().StartsWith("<repository"))
        {
            return line;
        }

        Match match = _commitRegex.Match(line);

        if (match.Success)
        {
            string commit = match.Groups[1].Value;
            string replacement = new('*', commit.Length);

            return line.Replace(commit, replacement);
        }

        return line;
    }

    public string ScrubVersion(string line)
    {
        if (!line.TrimStart().StartsWith("<version>"))
        {
            return line;
        }

        Match match = _versionRegex.Match(line);

        if (match.Success)
        {
            string version = match.Groups[1].Value;
            string replacement = new('*', version.Length);

            return line.Replace(version, replacement);
        }

        return line;
    }
}

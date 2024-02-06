using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            string replacement = new('*', count: 40); // Length chosen because it's the standard length of a git SHA. Any number will work, but it should be consistent.

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
            string replacement = new('*', count: 8); // Length chosen because it looks nice in diffs. Any number will work, but it should be consistent.

            return line.Replace(version, replacement);
        }

        return line;
    }

    public string ScrubRepositoryUrl(string line)
    {
        string leading = line.GetLeadingWhiteSpace();

        if (!XElementExtensions.TryParseForScrubbing(line, out XElement? element))
        {
            return line;
        }

        XAttribute? urlAttribute = element.Attribute("url");

        if (urlAttribute is null)
        {
            return line;
        }

        UriBuilder url = new(urlAttribute.Value);

        // Exclude non-GitHub URLs, and other formats like SSH
        if (!(url.Host == "github.com" && url.Scheme == "https"))
        {
            return line;
        }

        if (!url.Path.EndsWith(".git"))
        {
            url.Path += ".git";
            urlAttribute.SetValue(url.Uri.ToString());
        }

        line = leading + element.ToString(SaveOptions.DisableFormatting);

        return line;
    }
}

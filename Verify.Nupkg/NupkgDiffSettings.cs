using System.Text.RegularExpressions;

namespace Verify.Nupkg;

/// <summary>
/// Settings to control the behavior of <see cref="VerifyNupkg"/>.
/// </summary>
public class NupkgDiffSettings
{
    internal static readonly string ContextKey = "NupkgDiffSettings";

    public static readonly IReadOnlyCollection<Regex> DefaultExcludedFiles = new List<Regex>
    {
        new(@"^\[Content_Types\].xml$"),
        new(@"\.psmdcp$")
    };

    /// <summary>
    /// Gets regular expressions to use to exclude files from the diff.
    /// </summary>
    public ICollection<Regex> ExcludedFiles { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NupkgDiffSettings"/> class.
    /// </summary>
    public NupkgDiffSettings()
    {
        ExcludedFiles = DefaultExcludedFiles.ToList();
    }
}

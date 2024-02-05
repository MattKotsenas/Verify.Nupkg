using System.Text.RegularExpressions;

namespace Verify.Nupkg;

/// <summary>
/// Settings to control the behavior of <see cref="VerifyNupkg"/>.
/// </summary>
public class NupkgDiffSettings
{
    internal static readonly string ContextKey = "NupkgDiffSettings";

    /// <summary>
    /// Gets the default regular expressions to use to exclude files from the diff.
    /// </summary>
    /// <remarks>
    /// The default excluded files are:
    ///   - [Content_Types].xml
    ///   - .psmdcp
    ///   - _rels/.rels
    /// </remarks>
    public static readonly IReadOnlyCollection<Regex> DefaultExcludedFiles =
    [
        new(@"^\[Content_Types\].xml$"),
        new(@"\.psmdcp$"),
        new(@"^_rels[/\\].rels$")
    ];

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

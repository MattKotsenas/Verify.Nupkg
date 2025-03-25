﻿using Microsoft.Build.Framework;

using Task = Microsoft.Build.Utilities.Task;

namespace Verify.Nupkg.Tasks;

[ModuleInitializer]
public static void Initialize() =>
    VerifyNupkg.Initialize();

public class VerifyNuspec : Task
{
    /// <summary>
    /// If the project passed in a .nuspec file, this is the path to it
    /// </summary>
    public string? NuspecFileAbsolutePath { get; set; }

    /// <summary>
    /// If the project is using GenerateNuspec to generate the .nuspec file, this is the item group containing the
    /// output .nuspec file and .nupkg file.
    /// </summary>
    public ITaskItem[]? NuGetPackOutput { get; set; }

    [Required]
    public string BaselineFile { get; set; }

    /// <summary>
    /// Validate the .nuspec file against the baseline.
    /// </summary>
    /// <returns><see langword="true" />, if successful.</returns>
    public override bool Execute()
    {
        // Debugger.Launch();

        if (NuspecFileAbsolutePath is null && NuGetPackOutput is null)
        {
            LogWarning(warningCode: DiagnosticIds.Input.NotSpecified, "Both {0} and {1} not specified", nameof(NuspecFileAbsolutePath, nameof(NuGetPackOutput)));
            return true;
        }

        if (NuspecFileAbsolutePath is not null && !File.Exists(ErrorLog))
        {
            LogWarning(warningCode: DiagnosticIds.Input.NotFound, ".nuspec specified by {0} not found: {1}", nameof(NuspecFileAbsolutePath), NuspecFileAbsolutePath);
            return true;
        }

        string? generatedNuspec = NuGetPackOutput?.FirstOrDefault(o => o.ItemSpec.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))?.ItemSpec;
        if (NuGetPackOutput is not null && generatedNuspec is null)
        {
            LogWarning(warningCode: DiagnosticIds.Input.NotFound, "No .nuspec files listed in {0)}", nameof(NuGetPackOutput);
            return true;
        }

        try
        {
            VerifySettings settings = new();
            settings.ScrubNuspec(); // Scrub commit and other volatile information from nuspec

            using var verifier = new InnerVerifier(targetDirectory, name: "sample");
            verifier.VerifyFile(filePath).GetAwaiter().GetResult();

            // return VerifyFile(packagePath, settings);
        }
        catch (Exception e)
        {
            // TODO: If what?
            LogWarning(warningCode: DiagnosticIds.Baseline.Mismatch, "Baseline mismatch: {0}", baselineFile);


            LogWarning(warningCode: DiagnosticIds.Baseline.Unknown, "Unknown error: {0}", e.Message);
            return true;
        }
    }

    private void LogWarning(string warningCode, string message, params object[] messageArgs)
    {
        string helpLink = $"https://github.com/MattKotsenas/Verify.Nupkg/tree/{ThisAssembly.GitCommitId}/docs/{warningCode}.md";
        Log.LogWarning(subcategory: null, warningCode, helpKeyword: null, helpLink, file: null, lineNumber: 0, columnNumber: 0, endLineNumber: 0, endColumnNumber: 0, message, messageArgs);
    }
}
using Microsoft.Build.Framework;
using Task = Microsoft.Build.Utilities.Task;

namespace Verify.Nupkg.Tasks;

/// <summary>
/// MSBuild task to verify NuGet package contents against a baseline using Verify.
/// </summary>
public class VerifyNupkgTask : Task
{
    /// <summary>
    /// The package files (.nupkg and/or .snupkg) to verify.
    /// </summary>
    [Required]
    public ITaskItem[] PackageFiles { get; set; } = [];

    /// <summary>
    /// The directory where baseline files are stored.
    /// Defaults to the project directory if not specified.
    /// </summary>
    [Required]
    public string VerifyDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Validate the package files against their baselines.
    /// </summary>
    /// <returns><see langword="true" />, if successful.</returns>
    public override bool Execute()
    {
        if (PackageFiles.Length == 0)
        {
            LogWarning(warningCode: DiagnosticIds.Input.PackageNotSpecified, "No package file specified in {0}", nameof(PackageFiles));
            return true;
        }

        if (string.IsNullOrEmpty(VerifyDirectory))
        {
            LogWarning(warningCode: DiagnosticIds.Input.BaselineDirectoryNotSpecified, "{0} is not specified", nameof(VerifyDirectory));
            return true;
        }

        if (!Directory.Exists(VerifyDirectory))
        {
            Directory.CreateDirectory(VerifyDirectory);
        }

        if (!VerifyNupkg.Initialized)
        {
            VerifyNupkg.Initialize();
        }

        foreach (ITaskItem packageFile in PackageFiles)
        {
            string packagePath = packageFile.ItemSpec;

            if (!File.Exists(packagePath))
            {
                LogWarning(warningCode: DiagnosticIds.Input.PackageNotFound, "Package file not found: {0}", packagePath);
                continue;
            }

            VerifyPackage(packagePath);
        }

        return true;
    }

    private void VerifyPackage(string packagePath)
    {
        string packageName = Path.GetFileName(packagePath);

        try
        {
            // TODO: Expose AutoVerify as a setting
            VerifySettings settings = new();
            settings.ScrubNuspec();
            settings.DisableDiff();
            settings.DisableRequireUniquePrefix();
            VerifierSettings.OmitContentFromException();


            using FileStream stream = File.OpenRead(packagePath);
            Target target = new("nupkg", stream, packageName);

            using var verifier = new InnerVerifier(VerifyDirectory, packageName, settings);
            verifier.Verify(target).GetAwaiter().GetResult();

            Log.LogMessage(MessageImportance.Normal, "Package verified successfully: {0}", packageName);
        }
        catch (Exception ex) when (IsVerifyException(ex))
        {
            LogWarning(
                warningCode: DiagnosticIds.Baseline.Mismatch,
                file: packagePath,
                "Package baseline mismatch: {0}",
                packageName);
        }
        catch (Exception ex)
        {
            LogWarning(
                warningCode: DiagnosticIds.Baseline.Unknown,
                file: packagePath,
                "Error verifying package {0}: {1}",
                packageName,
                ex.Message);
        }
    }

    private static bool IsVerifyException(Exception ex)
    {
        // VerifyException is internal, so we check by type name
        return ex.GetType().Name == "VerifyException" ||
               ex.GetType().FullName?.StartsWith("VerifyTests.") == true;
    }

    private void LogWarning(string warningCode, string message, params object[] messageArgs)
    {
        LogWarning(warningCode, file: null, message, messageArgs);
    }

    private void LogWarning(string warningCode, string? file, string message, params object[] messageArgs)
    {
        string helpLink = $"https://github.com/MattKotsenas/Verify.Nupkg/tree/{ThisAssembly.GitCommitId}/docs/{warningCode}.md";
        Log.LogWarning(
            subcategory: null,
            warningCode,
            helpKeyword: null,
            helpLink,
            file,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            message,
            messageArgs);
    }
}
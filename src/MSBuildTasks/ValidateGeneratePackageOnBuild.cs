using Microsoft.Build.Framework;
using Microsoft.Build.Evaluation;
using System.Diagnostics;

namespace MSBuildTasks;

public class ValidateGeneratePackageOnBuild : Microsoft.Build.Utilities.Task
{
    [Required]
    public string? ProjectFile { get; set; }

    public string? AttachDebugger { get; set; }

    private Project GetProject()
    {
        Log.LogMessage(MessageImportance.Normal, $"Loading project file '{ProjectFile}' for validation.");
        var project = ProjectCollection.GlobalProjectCollection.LoadProject(ProjectFile);
        Log.LogMessage(MessageImportance.Low, "Successfully loaded project file.");

        return project;
    }

    public override bool Execute()
    {
        if (ShouldDebug())
        {
            Debugger.Launch();
        }

        Project project = GetProject();
        ProjectProperty property = project.GetProperty("GeneratePackageOnBuild");

        if (!bool.TryParse(property.EvaluatedValue, out bool generatePackageOnBuild) || !generatePackageOnBuild)
        {
            LogDiagnostic("VN001", ProjectFile!, "GeneratePackageOnBuild=true not detected. Ensure GeneratePackageOnBuild is set to true to avoid consuming a stale package.");
        }

        return !Log.HasLoggedErrors;
    }

    private void LogDiagnostic(string warningCode, string file, string message)
    {
        Log.LogWarning(
            subcategory: null,
            warningCode: warningCode,
            helpKeyword: null,
            file: file,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            message: message);
    }

    private bool ShouldDebug()
    {
        bool wasParsed = bool.TryParse(AttachDebugger, out bool debug);

        return wasParsed && debug;
    }
}

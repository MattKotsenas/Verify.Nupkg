using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace Verify.Benchmarks;

public class NuspecScrubbing
{
    private const string Nuspec =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd"">
  <metadata minClientVersion=""2.12"">
    <id>Newtonsoft.Json</id>
    <version>13.0.3</version>
    <title>Json.NET</title>
    <authors>James Newton-King</authors>
    <owners></owners>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <license type=""expression"">MIT</license>
    <icon>packageIcon.png</icon>
    <readme>README.md</readme>
    <projectUrl>https://www.newtonsoft.com/json</projectUrl>
    <iconUrl>https://www.newtonsoft.com/content/images/nugeticon.png</iconUrl>
    <description>Json.NET is a popular high-performance JSON framework for .NET</description>
    <copyright>Copyright © James Newton-King 2008</copyright>
    <tags>json</tags>
    <repository type=""git"" url=""https://github.com/JamesNK/Newtonsoft.Json"" commit=""0a2e291c0d9c0c7675d445703e51750363a549ef"" />
    <dependencies>
      <group targetFramework="".NETFramework2.0"" />
      <group targetFramework="".NETFramework3.5"" />
      <group targetFramework="".NETFramework4.0"" />
      <group targetFramework="".NETFramework4.5"" />
      <group targetFramework="".NETStandard1.0"">
        <dependency id=""Microsoft.CSharp"" version=""4.3.0"" exclude=""Build,Analyzers"" />
        <dependency id=""NETStandard.Library"" version=""1.6.1"" exclude=""Build,Analyzers"" />
        <dependency id=""System.ComponentModel.TypeConverter"" version=""4.3.0"" exclude=""Build,Analyzers"" />
        <dependency id=""System.Runtime.Serialization.Primitives"" version=""4.3.0"" exclude=""Build,Analyzers"" />
      </group>
      <group targetFramework="".NETStandard1.3"">
        <dependency id=""Microsoft.CSharp"" version=""4.3.0"" exclude=""Build,Analyzers"" />
        <dependency id=""NETStandard.Library"" version=""1.6.1"" exclude=""Build,Analyzers"" />
        <dependency id=""System.ComponentModel.TypeConverter"" version=""4.3.0"" exclude=""Build,Analyzers"" />
        <dependency id=""System.Runtime.Serialization.Formatters"" version=""4.3.0"" exclude=""Build,Analyzers"" />
        <dependency id=""System.Runtime.Serialization.Primitives"" version=""4.3.0"" exclude=""Build,Analyzers"" />
        <dependency id=""System.Xml.XmlDocument"" version=""4.3.0"" exclude=""Build,Analyzers"" />
      </group>
      <group targetFramework=""net6.0"" />
      <group targetFramework="".NETStandard2.0"" />
    </dependencies>
  </metadata>
</package>";

    private readonly VerifySettings _settings;
    private readonly Target _target;
    private readonly string _verifyFile = Path.Combine(Directory.GetCurrentDirectory(), "can-be-anything.nuspec");

    public NuspecScrubbing()
    {
        _settings = new VerifySettings();
        _settings.DisableRequireUniquePrefix();
        _settings.ScrubNuspec();
        _settings.DisableDiff();
        _settings.AutoVerify();

        _target = new Target(extension: "nuspec", Nuspec);
    }

    [Benchmark]
    public async Task Baseline()
    {
        Func<InnerVerifier, Task<VerifyResult>> verify = _ => _.Verify(_target);

        await new SettingsTask(_settings, async verifySettings =>
        {
            using var verifier = new InnerVerifier(Environment.CurrentDirectory, _verifyFile, verifySettings);
            return await verify(verifier);
        });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        string verifiedFile = _verifyFile.Replace(".nuspec", ".verified.nuspec");
        File.Delete(verifiedFile);
    }
}

public class Program
{
    // Use this to debug the harness itself
    //public static async Task Main()
    //{
    //    var benchmark = new NuspecScrubbing();
    //    await benchmark.Baseline();
    //    benchmark.Cleanup();
    //}

    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddDiagnoser(ExceptionDiagnoser.Default)
            .AddJob(Job
                 .MediumRun
                 .WithToolchain(InProcessNoEmitToolchain.Instance)); // Avoid Defender

        var summary = BenchmarkRunner.Run<NuspecScrubbing>(config);
    }
}

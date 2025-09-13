using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.GitVersioning;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Nerdbank.GitVersioning;
using Spectre.Console;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Build;
using Cake.Core.Diagnostics;
using Cake.Common.Tools.DotNet.Tool;
using Cake.Common.Tools.DotNet.Run;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string AppFileRoot { get; }
    public string ProjectName { get; set; } = "BTProgressHUD";
    public string Target { get; set; }
    public string BuildConfiguration { get; set; }
    public string ArtifactsDir { get; set; }
    public DirectoryPath OutputDir { get; set; }
    public VersionOracle VersionInfo { get; set; }
    public FilePath ProjectPath { get; set; }
    public DotNetVerbosity VerbosityDotNet { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        AppFileRoot = context.Argument("root", "..");
        Target = context.Argument("target", "Default");
        BuildConfiguration = context.Argument("configuration", "Release");
        ArtifactsDir = context.Argument("artifactsDir", $"{AppFileRoot}/artifacts");
        OutputDir = new DirectoryPath(ArtifactsDir);

        ProjectPath = new FilePath($"{AppFileRoot}/{ProjectName}/{ProjectName}.csproj");

        VersionInfo = context.GitVersioningGetVersion();

        var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

        AnsiConsole.Write(new FigletText(ProjectName));
        context.Information("Building version {0}, ({1}, {2}) using version {3} of Cake.",
            VersionInfo.SemVer2,
            BuildConfiguration,
            Target,
            cakeVersion);

        if (context.GitHubActions().Environment.PullRequest.IsPullRequest)
        {
            context.Information("PR HeadRef: {0}", context.GitHubActions().Environment.Workflow.HeadRef);
            context.Information("PR BaseRef: {0}", context.GitHubActions().Environment.Workflow.BaseRef);
        }

        context.Information("RefName: {0}", context.GitHubActions().Environment.Workflow.RefName);

        VerbosityDotNet = context.Log.Verbosity switch
        {
            Verbosity.Quiet => DotNetVerbosity.Quiet,
            Verbosity.Normal => DotNetVerbosity.Normal,
            Verbosity.Verbose => DotNetVerbosity.Detailed,
            Verbosity.Diagnostic => DotNetVerbosity.Diagnostic,
            _ => DotNetVerbosity.Minimal
        };
    }

    public DotNetBuildSettings GetDefaultBuildSettings(DotNetMSBuildSettings msBuildSettings = null)
    {
        msBuildSettings ??= GetDefaultDotNetMSBuildSettings();

        var settings = new DotNetBuildSettings
        {
            Configuration = BuildConfiguration,
            MSBuildSettings = msBuildSettings,
            Verbosity = VerbosityDotNet
        };

        return settings;
    }

    public DotNetMSBuildSettings GetDefaultDotNetMSBuildSettings()
    {
        var settings = new DotNetMSBuildSettings
        {
            Version = VersionInfo.SemVer2,
            PackageVersion = VersionInfo.NuGetPackageVersion,
            InformationalVersion = VersionInfo.AssemblyInformationalVersion
        };

        return settings;
    }
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"{context.AppFileRoot}/{context.ProjectName}*/**/bin");
        context.CleanDirectories($"{context.AppFileRoot}/{context.ProjectName}*/**/obj");
        context.CleanDirectories(context.OutputDir.FullPath);

        context.EnsureDirectoryExists(context.OutputDir);
    }
}

[TaskName("Restore")]
[IsDependentOn(typeof(CleanTask))]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore(context.ProjectPath.ToString());
    }
}


[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = context.GetDefaultBuildSettings();
        context.DotNetBuild(context.ProjectPath.ToString(), settings);
    }
}

[TaskName("GenerateSBOM")]
[IsDependentOn(typeof(BuildTask))]
public sealed class GenerateSbomTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ProcessArgumentBuilder PrepareSbomArguments(ProcessArgumentBuilder args)
        {
            args.Append("generate");
            args.Append("-b {0}", context.OutputDir.FullPath);
            args.Append("-bc {0}", context.AppFileRoot);
            args.Append("-nsb https://github.com/redth-org/BTProgressHUD");
            args.Append("-ps redth-org");
            args.Append("-pn BTProgressHUD");
            args.Append("-V Verbose");
            args.Append("-pv {0}", context.VersionInfo.SemVer2);
            return args;
        }

        var settings = new DotNetToolSettings
        {
            ArgumentCustomization = PrepareSbomArguments
        };

        context.DotNetTool("sbom-tool", settings);
    }
}

[TaskName("CopyPackages")]
[IsDependentOn(typeof(BuildTask))]
public sealed class CopyPackagesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var packagesDir = context.OutputDir.Combine("NuGet/");
        context.EnsureDirectoryExists(packagesDir);

        var nugetFiles = context.GetFiles($"{context.AppFileRoot}/{context.ProjectName}*/**/bin/{context.BuildConfiguration}/**/*.nupkg");
        context.CopyFiles(nugetFiles, packagesDir);
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(GenerateSbomTask))]
[IsDependentOn(typeof(CopyPackagesTask))]
public sealed class DefaultTask : FrostingTask<BuildContext>
{
}

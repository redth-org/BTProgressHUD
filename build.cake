#module nuget:https://api.nuget.org/v3/index.json?package=Cake.DotNetTool.Module&version=0.4.0
#tool dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=5.7.0
#tool nuget:https://api.nuget.org/v3/index.json?package=vswhere&version=2.8.4
#addin nuget:https://api.nuget.org/v3/index.json?package=Cake.Figlet&version=2.0.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = new FilePath("./BTProgressHUD.sln");
var artifactsDir = new DirectoryPath("./artifacts");
var gitVersionLog = new FilePath("./gitversion.log");

GitVersion versionInfo = null;

Setup(context => 
{
    versionInfo = context.GitVersion(new GitVersionSettings 
    {
        UpdateAssemblyInfo = true,
        OutputType = GitVersionOutput.Json,
        LogFilePath = gitVersionLog.MakeAbsolute(context.Environment)
    });

    var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

    Information(Figlet("BTProgressHUD"));
    Information("Building version {0}, ({1}, {2}) using version {3} of Cake.",
        versionInfo.SemVer,
        configuration,
        target,
        cakeVersion);
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./BTProgressHUD/bin");
    CleanDirectories("./BTProgressHUD/obj");

    EnsureDirectoryExists(artifactsDir);
});

FilePath msBuildPath;
Task("ResolveBuildTools")
    .WithCriteria(() => IsRunningOnWindows())
    .Does(() => 
{
    var vsWhereSettings = new VSWhereLatestSettings
    {
        IncludePrerelease = true,
        Requires = "Component.Xamarin"
    };

    var vsLatest = VSWhereLatest(vsWhereSettings);
    msBuildPath = (vsLatest == null)
        ? null
        : vsLatest.CombineWithFilePath("./MSBuild/Current/Bin/MSBuild.exe");

    if (msBuildPath != null)
        Information("Found MSBuild at {0}", msBuildPath.ToString());
});

Task("Restore")
    .IsDependentOn("ResolveBuildTools")
    .Does(() => 
    {
    var settings = GetDefaultBuildSettings()
        .WithTarget("Restore");
    MSBuild(sln, settings);
});

Task("Build")
    .IsDependentOn("ResolveBuildTools")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>  
{
    var settings = GetDefaultBuildSettings()
        .WithProperty("Version", versionInfo.SemVer)
        .WithProperty("PackageVersion", versionInfo.SemVer)
        .WithProperty("InformationalVersion", versionInfo.InformationalVersion)
        .WithProperty("NoPackageAnalysis", "True")
        .WithTarget("Build");

    if (IsRunningOnWindows())
    {
        var javaSdkDir = EnvironmentVariable("JAVA_HOME_8_X64");
        Information("Setting JavaSdkDirectory to: " + javaSdkDir);
        settings = settings.WithProperty("JavaSdkDirectory", javaSdkDir);
    }

    MSBuild(sln, settings);
});

Task("CopyArtifacts")
    .IsDependentOn("Build")
    .Does(() => 
{
    var nugetFiles = GetFiles("BTProgressHUD/bin/" + configuration + "/**/*.nupkg");
    CopyFiles(nugetFiles, artifactsDir);
    CopyFileToDirectory(gitVersionLog, artifactsDir);
});

Task("Default")
    .IsDependentOn("CopyArtifacts");

RunTarget(target);

MSBuildSettings GetDefaultBuildSettings()
{
    var settings = new MSBuildSettings 
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("/m"),
        ToolVersion = MSBuildToolVersion.VS2019
    };

    if (msBuildPath != null)
        settings.ToolPath = msBuildPath;

    return settings;
}
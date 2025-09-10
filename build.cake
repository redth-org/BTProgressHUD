#tool dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=5.12.0
#addin nuget:https://api.nuget.org/v3/index.json?package=Cake.Figlet&version=2.0.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = new FilePath("./BTProgressHUD/BTProgressHUD.csproj");
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

Task("Restore")
    .Does(() => 
{
    DotNetRestore(sln.ToString());
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>  
{
    var msBuildSettings = new DotNetMSBuildSettings
    {
        Version = versionInfo.SemVer,
        PackageVersion = versionInfo.SemVer,
        InformationalVersion = versionInfo.InformationalVersion
    };

    var settings = new DotNetBuildSettings
    {
         Configuration = configuration,
         MSBuildSettings = msBuildSettings
    };

    DotNetBuild(sln.ToString(), settings);
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

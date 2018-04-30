#tool "nuget:?package=OctopusTools"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target                      = Argument("target", "Default");
var buildConfiguration          = Argument("configuration", "Release");
var verbosity                   = Argument("verbosity", "Verbose");
var nugetFeed                   = Argument("nugetFeed", (string) null);
var apiKey                      = Argument("apiKey", (string) null);

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var project = "../source/DemoApp";

Setup(context =>
{
    if (BuildSystem.IsRunningOnTeamCity)
    {
        commitHash = Context.Environment.GetEnvironmentVariable("BUILD_VCS_NUMBER");
    }
    version = "1.1.1.1"; // todo: replace this with code that gets the version string
    Information("version is: " + version);    
});


Task("Clean")
    .Does(() =>
{
    CleanDirectories(GetDirectories(project + "/**/bin/"));
    CleanDirectories(GetDirectories(project + "/**/obj/"));
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(project);
});

Task("Update-Version-Info")
    .IsDependentOn("Restore")
    .Does(() =>
{
    if(BuildSystem.IsRunningOnTeamCity)
    {
        GetFiles("../src/**/*.csproj")
            .ToList()
            .ForEach(f => XmlPoke(f.FullPath, "/Project/PropertyGroup/FileVersion", version));
        GetFiles("../src/**/*.csproj")
            .ToList()
            .ForEach(f => XmlPoke(f.FullPath, "/Project/PropertyGroup/AssemblyVersion", version));
        GetFiles("../src/**/*.csproj")
            .ToList()
            .ForEach(f => 
            {
                string description = XmlPeek(f.FullPath, "/Project/PropertyGroup/Description/text()");
                XmlPoke(f.FullPath, "/Project/PropertyGroup/InformationalVersion", version + "-" + commitHash);
                XmlPoke(f.FullPath, "/Project/PropertyGroup/Description", description + "-" + commitHash);
            });
    }
    else
    {
        // CheckIfRequiredFileVersionAndAssemblyVersionAreSet(Context, GetFiles(srcDirectory.ToString() + "/**/*.csproj"));
    }
        
});

Task("Build")
    .IsDependentOn("Update-Version-Info")
    .Does(() => 
{
    DotNetCoreBuild(project, new DotNetCoreBuildSettings {
        Configuration = configuration
    });
});

Task("Test")
  .IsDependentOn("Build")
    .Does(() =>
{
    GetFiles("../tests/**/*.csproj")
        .ToList()
        .ForEach(f => DotNetCoreTest(f.FullPath));
    ;
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(()=>
{
    var settings = new DotNetCorePublishSettings
    {
        Framework = "netcoreapp2.0",
        Configuration = buildConfiguration,
        OutputDirectory = "./artifacts/"
     };

     DotNetCorePublish(project, settings);
});

Task("Pack")
    .IsDependentOn("Publish")
    .Does(() => 
{
    var nuGetPackSettings = new NuGetPackSettings 
    {
        Id                      = packageId,
        Version                 = version,
        Title                   = packageId,
        Authors                 = new[] {"<Author/Company name>"},
        Description             = "Deploy package for the <my thing>",
        Summary                 = "",
        ProjectUrl              = new Uri("https://<eg. url to the project git repo>"),
        Files                   = new [] {
                                            new NuSpecContent {Source = "**/*.*", Target = ""},
                                            },
        BasePath                = "./artifacts",
        OutputDirectory         = ".",
        NoPackageAnalysis       = true
    };
    NuGetPack(nuGetPackSettings);
});

Task("OctoPush")
    .IsDependentOn("Pack")
    .WithCriteria(BuildSystem.IsRunningOnTeamCity)
    .Does(() => 
{
    OctoPush("https://deploy.mycompany.com", apiKey, new FilePath($"./{packageId}.{version}.nupkg"),
      new OctopusPushSettings {
        ReplaceExisting = true
      });
});


Task("OctoRelease")
  .IsDependentOn("OctoPush")
  .Does(() => {
    OctoCreateRelease("My.Package.Name", new CreateReleaseSettings {
        Server = "https://deploy.mycompany.com",
        ApiKey = apiKey,
        ReleaseNumber = version
      });
  });

Task("Default")
  .IsDependentOn("Test");

RunTarget(target);

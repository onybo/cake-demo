#addin  "Cake.FileHelpers"
using System.Linq;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var project = "../source/DemoApp";

Setup(context =>
{
    if (BuildSystem.IsRunningOnTeamCity)
    {
        Information("Running on TeamCity");
    }
    else
    {
        Information("Not running on TeamCity");
    }
    Information(FileReadLines(new FilePath("./test.txt")).FirstOrDefault());
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

Task("Build")
    .IsDependentOn("Restore")
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

Task("Default")
  .IsDependentOn("Test");

RunTarget(target);

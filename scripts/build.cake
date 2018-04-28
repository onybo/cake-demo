var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var project = "../source/DemoApp";

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

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);

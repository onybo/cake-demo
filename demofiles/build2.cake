Task("Test")
  .IsDependentOn("Build")
    .Does(() =>
{
    GetFiles("../tests/**/*.csproj")
        .ToList()
        .ForEach(f => DotNetCoreTest(f.FullPath));
    ;
});

// loading scripts
    #load "helper.cake"
    
    var message = SayHelloNumber(Context, 10);
    Information(message);


#addin  "Cake.FileHelpers"
using System.Linq;

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


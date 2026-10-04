#:package Bullseye@6.1.0
#:package SimpleExec@13.0.0

using static Bullseye.Targets;
using static SimpleExec.Command;

var version = default(string);

Target("clean", () =>
{
    Run("dotnet", "clean --configuration Release");

    if (Directory.Exists("artifacts"))
    {
        Directory.Delete("artifacts", true);
    }
});

Target("version", () =>
{
    version = Environment.GetEnvironmentVariable("GITHUB_REF_TYPE") is "tag"
        ? Environment.GetEnvironmentVariable("GITHUB_REF_NAME")?.TrimStart('v')
        : $"0.0.{Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER")}";
});

Target("restore", dependsOn: ["clean"], () =>
{
    Run("dotnet", "restore");
});

Target("build", dependsOn: ["restore", "version"], () =>
{
    Run("dotnet", $"build --configuration Release --no-restore --property Version={version}");
});

Target("test", dependsOn: ["build"], () =>
{
    Run("dotnet", "test --configuration Release --no-restore --no-build");
});

Target("package", dependsOn: ["build", "test", "version"], () =>
{
    Run("dotnet", $"pack --configuration Release --no-restore --no-build --output artifacts --property Version={version}");
});

Target("publish", dependsOn: ["package"], () =>
{
    var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");

    Run("dotnet", $"nuget push artifacts/*.nupkg --api-key {apiKey} --source https://api.nuget.org/v3/index.json");
});

Target("default", dependsOn: ["package"]);

await RunTargetsAndExitAsync(args);

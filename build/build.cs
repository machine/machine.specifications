#:package Bullseye@6.1.0
#:package SimpleExec@13.0.0

using static Bullseye.Targets;
using static SimpleExec.Command;

var version = new GitVersion();

Target("clean", () =>
{
    Run("dotnet", "clean --configuration Release");

    if (Directory.Exists("artifacts"))
    {
        Directory.Delete("artifacts", true);
    }
});

Target("restore", dependsOn: ["clean"], () =>
{
    Run("dotnet", "restore");
});

Target("build", dependsOn: ["restore"], () =>
{
    Run("dotnet", "build " +
                  "--no-restore " +
                  "--configuration Release " +
                  $"--property Version={version.SemVer} " +
                  $"--property AssemblyVersion={version.AssemblySemVer} " +
                  $"--property FileVersion={version.AssemblySemFileVer} " +
                  $"--property InformationalVersion={version.InformationalVersion}");
});

Target("test", dependsOn: ["build"], () =>
{
    Run("dotnet", "test --configuration Release --no-restore --no-build");
});

Target("package", dependsOn: ["build", "test"], () =>
{
    Run("dotnet", $"pack --configuration Release --no-restore --no-build --output artifacts --property Version={version.SemVer}");
});

Target("publish", dependsOn: ["package"], () =>
{
    var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");

    Run("dotnet", $"nuget push {Path.Combine("artifacts", "*.nupkg")} --api-key {apiKey} --source https://api.nuget.org/v3/index.json");
});

Target("default", dependsOn: ["package"]);

await RunTargetsAndExitAsync(args);

public class GitVersion
{
    public string SemVer { get; } = Environment.GetEnvironmentVariable("GitVersion_SemVer") ?? "0.1.0";

    public string AssemblySemVer { get; } = Environment.GetEnvironmentVariable("GitVersion_AssemblySemVer") ?? "0.1.0";

    public string AssemblySemFileVer { get; } = Environment.GetEnvironmentVariable("GitVersion_AssemblySemFileVer") ?? "0.1.0";

    public string InformationalVersion { get; } = Environment.GetEnvironmentVariable("GitVersion_InformationalVersion") ?? "0.1.0";
}

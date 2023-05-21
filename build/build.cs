using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using static Bullseye.Targets;
using static SimpleExec.Command;

var version = await GetGitVersion();

Target("clean", () =>
{
    Run("dotnet", "clean --configuration Release");

    if (Directory.Exists("artifacts"))
    {
        Directory.Delete("artifacts", true);
    }
});

Target("restore", DependsOn("clean"), () =>
{
    Run("dotnet", "restore");
});

Target("build", DependsOn("restore"), () =>
{
    Run("dotnet", "build " +
                  "--no-restore " +
                  "--configuration Release " +
                  $"--property Version={version.SemVer} " +
                  $"--property AssemblyVersion={version.AssemblySemVer} " +
                  $"--property FileVersion={version.AssemblySemFileVer} " +
                  $"--property InformationalVersion={version.InformationalVersion}");
});

Target("test", DependsOn("build"), () =>
{
    Run("dotnet", "test --configuration Release --no-restore --no-build");
});

Target("package", DependsOn("build", "test"), () =>
{
    Run("dotnet", $"pack --configuration Release --no-restore --no-build --output artifacts --property Version={version.SemVer}");
});

Target("publish", DependsOn("package"), () =>
{
    var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");

    //Run("dotnet", $"nuget push {Path.Combine("artifacts", "*.nupkg")} --api-key {apiKey} --source https://api.nuget.org/v3/index.json");
});

Target("default", DependsOn("package"));

await RunTargetsAndExitAsync(args);

async Task<GitVersion> GetGitVersion()
{
    Run("dotnet", "tool restore");

    var (value, _) = await ReadAsync("dotnet", "dotnet-gitversion");

    return JsonSerializer.Deserialize<GitVersion>(value);
}

public class GitVersion
{
    public string SemVer { get; set; }

    public string AssemblySemVer { get; set; }

    public string AssemblySemFileVer { get; set; }

    public string InformationalVersion { get; set; }

    public string PreReleaseTag { get; set; }
}

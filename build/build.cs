using System;
using System.IO;
using System.Text.Json;
using static Bullseye.Targets;
using static SimpleExec.Command;

public class build
{
    public static void Main(string[] args)
    {
        var version = GetGitVersion();

        Target("clean", () =>
        {
            Run("dotnet", "clean");

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
                          $"/p:Version={version.SemVer} " +
                          $"/p:AssemblyVersion={version.AssemblySemVer} " +
                          $"/p:FileVersion={version.AssemblySemFileVer} " +
                          $"/p:InformationalVersion={version.InformationalVersion}");
        });

        Target("test", DependsOn("build"), () =>
        {
            Run("dotnet", "test --configuration Release --no-restore --no-build");
        });

        Target("package", DependsOn("build", "test"), () =>
        {
            Run("dotnet", $"pack --configuration Release --no-restore --no-build --output artifacts /p:Version={version.SemVer}");
        });

        Target("publish", DependsOn("package"), () =>
        {
            var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");

            Run("dotnet", $"nuget push {Path.Combine("artifacts", "*.nupkg")} --api-key {apiKey} --source https://api.nuget.org/v3/index.json");
        });

        Target("default", DependsOn("package"));

        RunTargetsAndExit(args);
    }

    private static GitVersion GetGitVersion()
    {
        Run("dotnet", "tool restore");

        var value = Read("dotnet", "dotnet-gitversion");

        return JsonSerializer.Deserialize<GitVersion>(value);
    }
}

public class GitVersion
{
    public string SemVer { get; set; }

    public string AssemblySemVer { get; set; }

    public string AssemblySemFileVer { get; set; }

    public string InformationalVersion { get; set; }

    public string PreReleaseTag { get; set; }
}

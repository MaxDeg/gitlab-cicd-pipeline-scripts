#!/usr/bin/dotnet run

#:include helpers.cs

using System.Text.Json;

namespace Targets;

public class VersionTarget() : Target("calculate version")
{
    private const string GitVersionConfigFileName = "GitVersion.yaml";

    public override async Task Run()
    {
        var (stdOut, stdErr) = await ReadAsync("dotnet-gitversion", "/output json");
        if (!string.IsNullOrEmpty(stdErr))
        {
            throw new Exception(stdErr);
        }

        var jsonDoc = JsonDocument.Parse(stdOut);
        // jsonDoc.RootElement.GetProperty("SemVer").GetString();
        Console.WriteLine(stdOut);
    }

    private Task InstallGitVersion() =>
        RunAsync("dotnet", "tool install --global GitVersion.Tool");

    private async Task EnsureGitVersionConfiguration()
    {
        if (!File.Exists(GitVersionConfigFileName))
        {
            await File.WriteAllTextAsync(
                GitVersionConfigFileName,
                """
                mode: ContinuousDeployment
                """);
        }
    }
}

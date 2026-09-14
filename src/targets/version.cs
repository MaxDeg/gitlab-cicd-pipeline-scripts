#!/usr/bin/dotnet run

#:include helpers.cs

using System.Text.Json;

namespace Targets;

public interface IVersionOptions
{
    DirectoryInfo WorkingDirectory { get; }
}

public class VersionTarget
    : RegistrableTarget<VersionTarget, IVersionOptions>, ITarget<IVersionOptions>
{
    private const string GitVersionConfigFileName = "GitVersion.yml";

    public static string Name => "calculate version";

    public static async Task RunTarget(IVersionOptions options)
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

    private static Task InstallGitVersion() =>
        RunAsync("dotnet", "tool install --global GitVersion.Tool");

    private static async Task EnsureGitVersionConfiguration()
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

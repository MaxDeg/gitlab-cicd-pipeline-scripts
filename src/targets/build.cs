#!/usr/bin/dotnet run

#:include restore.cs
#:include version.cs

namespace Targets;

public interface IDotNetOptions
{
    DirectoryInfo WorkingDirectory { get; }

    FileInfo Solution { get; }
}

public class DotNetBuildTarget
    : RegistrableTarget<DotNetBuildTarget, IDotNetOptions>, ITarget<IDotNetOptions>
{
    public static string Name => "dotnet build";

    public static async Task RunTarget(IDotNetOptions options)
    {
        await RunAsync("dotnet", $"build {options.Solution.Name}", workingDirectory: options.WorkingDirectory.FullName);
    }
}

// public class NpmBuildTarget() : Target("npm build")
// {
//     // public override string[] GetDependencies(string[] dependsOn) =>
//     // [
//     //     RegisterTarget<NpmRestoreTarget>(dependsOn),
//     // ];

//     public override Task Run()
//     {
//         return Task.Delay(TimeSpan.FromSeconds(5));
//     }
// }

#!/usr/bin/dotnet run

#:include helpers.cs

namespace Targets;

public interface IRestoreOptions
{
    DirectoryInfo WorkingDirectory { get; }

    FileInfo Solution { get; }
}

public class DotNetRestoreTarget
    : RegistrableTarget<DotNetRestoreTarget, IRestoreOptions>, ITarget<IRestoreOptions>
{
    public static string Name => "dotnet restore";

    public static async Task RunTarget(IRestoreOptions options)
    {
        await RunAsync(
            "dotnet",
            $"restore {options.Solution.Name}",
            workingDirectory: options.WorkingDirectory.FullName);
    }
}

// public class NpmRestoreTarget() : Target("npm restore")
// {
//     public override Task Run()
//     {
//         return Task.Delay(TimeSpan.FromSeconds(2));
//     }
// }

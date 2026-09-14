#!/usr/bin/dotnet run

#:include restore.cs
#:include version.cs

namespace Targets;

public class BuildTarget(FileInfo solution) : Target("dotnet build")
{
    public override async Task Run()
    {
        //Console.WriteLine($"Calculated version : {VersionTarget.CalculatedVersion}");
        await RunAsync("dotnet", $"build {solution.Name}", workingDirectory: solution.Directory.FullName);
    }
}

public class NpmBuildTarget() : Target("npm build")
{
    // public override string[] GetDependencies(string[] dependsOn) =>
    // [
    //     RegisterTarget<NpmRestoreTarget>(dependsOn),
    // ];

    public override Task Run()
    {
        return Task.Delay(TimeSpan.FromSeconds(5));
    }
}

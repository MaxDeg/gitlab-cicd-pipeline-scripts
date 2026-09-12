#!/usr/bin/dotnet run

#:include restore.cs
#:include version.cs

namespace Targets;

public class BuildTarget() : Target("dotnet build")
{
    public override string[] GetDependencies(string[] dependsOn) =>
    [
        Target<VersionTarget>(dependsOn),
        Target<DotnetRestoreTarget>(dependsOn),
        Target<NpmBuildTarget>(dependsOn),
    ];

    public override async Task Run()
    {
        _ = VersionTarget.CalculatedVersion;
        // await RunAsync("dotnet", "build");
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

public class NpmBuildTarget() : Target("npm build")
{
    public override string[] GetDependencies(string[] dependsOn) =>
    [
        Target<NpmRestoreTarget>(dependsOn),
    ];

    public override Task Run()
    {
        return Task.Delay(TimeSpan.FromSeconds(5));
    }
}

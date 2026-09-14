#!/usr/bin/dotnet run

#:include helpers.cs

namespace Targets;

public class DotnetRestoreTarget(FileInfo solution) : Target("dotnet restore")
{
    public override async Task Run()
    {
        await RunAsync("dotnet", $"restore {solution.Name}", workingDirectory: solution.Directory.FullName);
    }
}

public class NpmRestoreTarget() : Target("npm restore")
{
    public override Task Run()
    {
        return Task.Delay(TimeSpan.FromSeconds(2));
    }
}

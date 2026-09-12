#!/usr/bin/dotnet run

#:include helpers.cs

namespace Targets;

public class DotnetRestoreTarget() : Target("dotnet restore")
{
    public override async Task Run()
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        //await RunAsync("dotnet", "restore");
    }
}

public class NpmRestoreTarget() : Target("npm restore")
{
    public override Task Run()
    {
        return Task.Delay(TimeSpan.FromSeconds(2));
    }
}

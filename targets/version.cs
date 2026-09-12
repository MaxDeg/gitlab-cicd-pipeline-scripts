#!/usr/bin/dotnet run

#:include helpers.cs

namespace Targets;

public class VersionTarget() : Target("calculate version")
{
    private static string? _version;

    public static string CalculatedVersion
    {
        get => _version ?? throw new InvalidOperationException("Version target didn't run. Check your dependencies!");
    }

    public override async Task Run()
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        _version = "1.1.3";
        //await RunAsync("dotnet", "restore");
    }
}

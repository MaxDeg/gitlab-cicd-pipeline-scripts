#!/usr/bin/dotnet run

#:package Bullseye

namespace Targets;

public abstract class Target(string name)
{
    public string Name { get; } = name;

    public virtual string[] GetDependencies(string[] dependsOn) => [];

    public virtual Task Run() => Task.CompletedTask;
}

public static class Helpers
{
    public static string Target<TTarget>(string[]? dependsOn = null)
        where TTarget : Target, new()
    {
        var target = new TTarget();
        Bullseye.Targets.Target(
            target.Name,
            [.. target.GetDependencies(dependsOn ?? []), .. dependsOn ?? []],
            target.Run);

        return target.Name;
    }
}

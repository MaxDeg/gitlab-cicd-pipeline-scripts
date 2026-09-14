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
    public static string RegisterTarget<TTarget>(string[]? dependsOn = null)
        where TTarget : Target, new()
    {
        var target = new TTarget();
        var dependencies = target.GetDependencies(dependsOn ?? []);

        Bullseye.Targets.Target(
            target.Name,
            [.. dependencies, .. dependsOn ?? []],
            target.Run);

        return target.Name;
    }

    public static string RegisterTarget(Target target, string[]? dependsOn = null)
    {
        var dependencies = target.GetDependencies(dependsOn ?? []);

        Bullseye.Targets.Target(
            target.Name,
            [.. dependencies, .. dependsOn ?? []],
            target.Run);

        return target.Name;
    }
}

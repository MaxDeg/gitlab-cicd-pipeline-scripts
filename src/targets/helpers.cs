#!/usr/bin/dotnet run

#:package Bullseye

namespace Targets;

public interface ITarget<TOptions>
{
    static abstract string Name { get; }

    static virtual string? Description => null;

    static abstract Task RunTarget(TOptions options);
}

public abstract class RegistrableTarget<TTarget, TOptions>
    where TTarget : ITarget<TOptions>
{
    public static string Configure(TOptions options, string[]? dependsOn = null)
    {
        Bullseye.Targets.Target(
            TTarget.Name,
            TTarget.Description ?? "",
            dependsOn ?? [],
            () => TTarget.RunTarget(options));

        return TTarget.Name;
    }
}

public interface ITarget<TOptions, TInput>
{
    static abstract string Name { get; }

    static virtual string? Description => null;

    static abstract Task RunTarget(TOptions options, TInput input);
}

public abstract class RegistrableTarget<TTarget, TOptions, TInput>
    where TTarget : ITarget<TOptions, TInput>
{
    public static string Configure(TOptions options, string[]? dependsOn = null)
    {
        Bullseye.Targets.Target(
            TTarget.Name,
            TTarget.Description ?? "",
            dependsOn ?? [],
            [],
            input => TTarget.RunTarget(options, input));

        return TTarget.Name;
    }
}

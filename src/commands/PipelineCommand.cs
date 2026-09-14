#!/usr/bin/dotnet run

#:package Bullseye
#:package System.CommandLine

using System.CommandLine;

namespace Commands;

public abstract class PipelineCommand : System.CommandLine.RootCommand
{
    public PipelineCommand(string description, Option[] options)
        : base(description)
    {
        SetAction(Run);
        foreach (var opts in options)
        {
            Options.Add(opts);
        }
    }

    protected abstract string[] RegisterTargets(ParseResult commandLine);

    private async Task Run(ParseResult commandLine)
    {
        var targets = RegisterTargets(commandLine);
        Target("default", dependsOn: targets);

        await RunTargetsAndExitAsync(["--parallel"], ex => ex is ExitCodeException);
    }

    public async Task<int> Invoke(IReadOnlyList<string> args)
    {
        return await Parse(args).InvokeAsync();
    }
}

public static class RootCommandExtensions
{
    extension(RootCommand rootCommand)
    {
        public RootCommand AddCommand<TCommand>() where TCommand : PipelineCommand, new()
        {
            rootCommand.Subcommands.Add(new TCommand());
            return rootCommand;
        }
    }
}

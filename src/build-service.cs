#!/usr/bin/dotnet run

#:package Bullseye
#:package SimpleExec
#:package System.CommandLine

#:include targets/build.cs
#:include commands/PipelineCommand.cs

using Bullseye;
using Commands;
using System.CommandLine;
using System.CommandLine.Parsing;

/*
var solutionOpts = Option<FileInfo>("--solution");

var parseResult = ParseArguments(args);

var solution = parseResult.GetRequiredValue(solutionOpts);

var restoreTarget = Target(a => new DotnetRestoreTarget(solution));
var versionTarget = Target(_ => new VersionTarget());

DefaultTarget(a => new BuildTarget(solution), [restoreTarget, versionTarget]);

return Run(args);
*/



return await new BuildServiceCommand().Invoke(args);

public class BuildServiceCommand()
    : PipelineCommand(
        "build the current repository",
        [SolutionOpts])
{
    private static readonly Option<FileInfo> SolutionOpts = new("--solution");

    protected override string[] RegisterTargets(ParseResult commandLine)
    {
        var solution = commandLine.GetRequiredValue(SolutionOpts);

        var restoreTarget = RegisterTarget(new DotnetRestoreTarget(solution));
        var versionTarget = RegisterTarget(new VersionTarget());
        var buildTarget = RegisterTarget(new BuildTarget(solution), [restoreTarget, versionTarget]);

        return [buildTarget];
    }
}

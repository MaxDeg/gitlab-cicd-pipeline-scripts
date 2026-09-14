#!/usr/bin/dotnet run

#:package Bullseye
#:package SimpleExec
#:package System.CommandLine

#:include targets/build.cs
#:include commands/PipelineCommand.cs

using Commands;
using static Commands.PipelineCommand;

var solutionOpts = Option<FileInfo>("--solution");

var parseResult = ParseArguments(args);
var options = new BuildServiceOptions
{
    WorkingDirectory = parseResult.GetRequiredValue(solutionOpts).Directory!,
    Solution = parseResult.GetRequiredValue(solutionOpts),
};

var versionTarget = VersionTarget.Configure(options);
var restoreTarget = DotNetRestoreTarget.Configure(options);
var buildTarget = DotNetBuildTarget.Configure(options, dependsOn: [versionTarget, restoreTarget]);

await RunTargets([buildTarget]);

public sealed class BuildServiceOptions : IRestoreOptions, IVersionOptions, IDotNetOptions
{
    public required DirectoryInfo WorkingDirectory { get; init; }

    public required FileInfo Solution { get; init; }
}

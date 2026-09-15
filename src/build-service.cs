#!/usr/bin/dotnet run

#:property TargetFramework=net10.0
#:property PublishAot=false

#:project PipelineTargets/PipelineTargets.csproj

var solutionOpts = Option<FileInfo>("--solution");

var parseResult = ParseArguments(args);
var options = new BuildServiceOptions
{
    WorkingDirectory = parseResult.GetRequiredValue(solutionOpts).Directory!,
    Solution = parseResult.GetRequiredValue(solutionOpts),
    ProjectKey = "TestProject",
    Token = "1234",
};

var customizeSolutionBuild = CustomizeSolutionBuildTarget.Configure(options);
var version = VersionTarget.Configure(options);
var restore = DotNetRestoreTarget.Configure(options, dependsOn: [customizeSolutionBuild]);
var sonarBegin = SonarBeginTarget.Configure(options, dependsOn: [restore]);
var build = DotNetBuildTarget.Configure(options, dependsOn: [version, restore, sonarBegin]);
var test = DotNetTestTarget.Configure(options, dependsOn: [build]);
var sonarEnd = SonarEndTarget.Configure(options, dependsOn: [sonarBegin, test]);

await RunTargets([sonarEnd]);

public sealed class BuildServiceOptions : ITargetOptions,
    IDotNetOptions, IDotNetRestoreOptions, IDotNetBuildOptions, IDotNetTestOptions,
    IVersionOptions,
    ISonarBeginOptions, ISonarEndOptions,
    ICustomizeSolutionBuildOptions
{
    public required DirectoryInfo WorkingDirectory { get; init; }

    public required FileInfo Solution { get; init; }

    public required string ProjectKey { get; init; }

    public required string Token { get; init; }

    public string? PropsFile { get; } = null;

    public string? TargetsFile { get; } = null;
}

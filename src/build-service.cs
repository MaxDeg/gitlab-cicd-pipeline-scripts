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

var customizeSolutionBuildTarget = CustomizeSolutionBuildTarget.Configure(options);
var versionTarget = VersionTarget.Configure(options);
var restoreTarget = DotNetRestoreTarget.Configure(options, dependsOn: [customizeSolutionBuildTarget]);
var sonarBeginTarget = SonarBeginTarget.Configure(options, dependsOn: [restoreTarget]);
var buildTarget = DotNetBuildTarget.Configure(options, dependsOn: [versionTarget, restoreTarget, sonarBeginTarget]);
var testTarget = DotNetTestTarget.Configure(options, dependsOn: [buildTarget]);
var sonarEndTarget = SonarEndTarget.Configure(options, dependsOn: [sonarBeginTarget, testTarget]);

await RunTargets([sonarEndTarget]);

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

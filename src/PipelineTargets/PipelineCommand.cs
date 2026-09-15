using System.CommandLine;

namespace PipelineTargets;

public class PipelineCommand : RootCommand
{
    private static readonly PipelineCommand Instance = [];

    public static Option<T> Option<T>(string name)
    {
        var opts = new Option<T>(name);
        Instance.Options.Add(opts);

        return opts;
    }

    public static ParseResult ParseArguments(string[] args)
    {
        return Instance.Parse(args);
    }

    public static Task RunTargets(string[] targets)
    {
        Bullseye.Targets.Target("default", dependsOn: targets);
        return Bullseye.Targets.RunTargetsAndExitAsync(["--parallel"], ex => ex is ExitCodeException);
    }
}

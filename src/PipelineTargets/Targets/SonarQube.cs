namespace PipelineTargets.Targets;

public interface ISonarBeginOptions : ITargetOptions
{
    string ProjectKey { get; }

    string Token { get; }
}

public class SonarBeginTarget : ConfigurableTarget<SonarBeginTarget, ISonarBeginOptions>,
    ITarget<ISonarBeginOptions>
{
    public static string Name => "sonar begin";

    public static Task RunTarget(ISonarBeginOptions options)
    {
        // await RunDnx("dotnet-sonarscanner", [
        //     "begin",
        //     "/key", options.ProjectKey,
        //     "/d:sonar.token", options.Token,
        //     ],
        //     secrets: [options.Token],
        //     workingDirectory: options.WorkingDirectory);
        return Task.CompletedTask;
    }
}

public interface ISonarEndOptions;

public class SonarEndTarget : ConfigurableTarget<SonarEndTarget, ISonarEndOptions>,
    ITarget<ISonarEndOptions>
{
    public static string Name => "sonar end";

    public static Task RunTarget(ISonarEndOptions options)
    {
        return Task.CompletedTask;
    }
}

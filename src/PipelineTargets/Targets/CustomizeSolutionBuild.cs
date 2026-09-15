namespace PipelineTargets.Targets;

public interface ICustomizeSolutionBuildOptions : IDotNetOptions
{
    string? PropsFile { get; }

    string? TargetsFile { get; }
}

public class CustomizeSolutionBuildTarget : ConfigurableTarget<CustomizeSolutionBuildTarget, ICustomizeSolutionBuildOptions>,
    ITarget<ICustomizeSolutionBuildOptions>
{
    public static string Name => "customize solution build";

    public static Task RunTarget(ICustomizeSolutionBuildOptions options)
    {
        return Task.CompletedTask;
    }
}

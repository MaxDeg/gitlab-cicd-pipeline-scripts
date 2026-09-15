namespace PipelineTargets.Targets;

public interface IDotNetTestOptions : IDotNetOptions;

public class DotNetTestTarget : ConfigurableTarget<DotNetTestTarget, IDotNetTestOptions>,
    ITarget<IDotNetTestOptions>
{
    public static string Name => "dotnet test";

    public static async Task RunTarget(IDotNetTestOptions options)
    {
        await Dotnet("test", [
            options.Solution.Name,
            "--no-build",
            ],
            options.WorkingDirectory);
    }
}

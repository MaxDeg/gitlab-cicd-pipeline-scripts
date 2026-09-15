namespace PipelineTargets.Targets;

public interface IDotNetRestoreOptions : IDotNetOptions;

public class DotNetRestoreTarget : ConfigurableTarget<DotNetRestoreTarget, IDotNetRestoreOptions>,
    ITarget<IDotNetRestoreOptions>
{
    public static string Name => "dotnet restore";

    public static async Task RunTarget(IDotNetRestoreOptions options)
    {
        await Dotnet(
            "restore",
            [options.Solution.Name],
            options.WorkingDirectory);
    }
}

// public class NpmRestoreTarget() : Target("npm restore")
// {
//     public override Task Run()
//     {
//         return Task.Delay(TimeSpan.FromSeconds(2));
//     }
// }

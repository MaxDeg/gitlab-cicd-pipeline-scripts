namespace PipelineTargets.Targets;

public interface IDotNetBuildOptions : IDotNetOptions;

public class DotNetBuildTarget : ConfigurableTarget<DotNetBuildTarget, IDotNetBuildOptions>,
    ITarget<IDotNetBuildOptions>
{
    public static string Name => "dotnet build";

    public static async Task RunTarget(IDotNetBuildOptions options)
    {
        await Dotnet("build", [
            options.Solution.Name,
            "--no-restore",
            "-c", "Release",
            ],
            options.WorkingDirectory);
    }
}

// public class NpmBuildTarget() : Target("npm build")
// {
//     // public override string[] GetDependencies(string[] dependsOn) =>
//     // [
//     //     RegisterTarget<NpmRestoreTarget>(dependsOn),
//     // ];

//     public override Task Run()
//     {
//         return Task.Delay(TimeSpan.FromSeconds(5));
//     }
// }

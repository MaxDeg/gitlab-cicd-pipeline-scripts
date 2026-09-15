namespace PipelineTargets;

public abstract class ConfigurableTarget<TTarget, TOptions>
    where TTarget : ITarget<TOptions>
{
    public static string Configure(TOptions options, string[]? dependsOn = null)
    {
        Bullseye.Targets.Target(
            TTarget.Name,
            TTarget.Description ?? "",
            dependsOn ?? [],
            () => TTarget.RunTarget(options));

        return TTarget.Name;
    }
}

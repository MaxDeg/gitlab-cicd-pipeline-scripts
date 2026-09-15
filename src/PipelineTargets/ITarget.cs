namespace PipelineTargets;

public interface ITarget<TOptions>
{
    static abstract string Name { get; }

    static virtual string? Description => null;

    static abstract Task RunTarget(TOptions options);
}

public interface ITarget<TOptions, TInput>
{
    static abstract string Name { get; }

    static virtual string? Description => null;

    static abstract Task RunTarget(TOptions options, TInput input);
}

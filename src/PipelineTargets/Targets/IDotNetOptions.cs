namespace PipelineTargets.Targets;

public interface IDotNetOptions : ITargetOptions
{
    FileInfo Solution { get; }
}

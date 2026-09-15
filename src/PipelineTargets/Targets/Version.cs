namespace PipelineTargets.Targets;

public interface IVersionOptions : IDotNetOptions;

public class VersionTarget : ConfigurableTarget<VersionTarget, IVersionOptions>,
    ITarget<IVersionOptions>
{
    private const string GitVersionConfigFileName = "GitVersion.yml";

    public static string Name => "calculate version";

    public static async Task RunTarget(IVersionOptions options)
    {
        await EnsureGitVersionConfiguration();

        var (stdOut, stdErr) = await Dnx("gitversion.tool", [
            "/output", "json",
            "/updateprojectfiles",
            "/verbosity", "normal",
        ],
        options.WorkingDirectory);

        if (!string.IsNullOrEmpty(stdErr))
        {
            throw new Exception(stdErr);
        }

        Console.WriteLine(stdOut);
    }

    private static async Task EnsureGitVersionConfiguration()
    {
        if (!File.Exists(GitVersionConfigFileName))
        {
            await File.WriteAllTextAsync(
                GitVersionConfigFileName,
                """
                mode: ContinuousDeployment
                """);
        }
    }
}

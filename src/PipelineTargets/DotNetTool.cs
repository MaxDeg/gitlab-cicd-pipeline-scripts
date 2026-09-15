namespace PipelineTargets;

public static class DotNetTool
{
    public static Task<(string StandardOutput, string StandardError)> Dnx(
        string toolName,
        string[] arguments,
        DirectoryInfo workingDirectory)
    {
        return ReadAsync("dnx", [toolName, "-y", .. arguments], workingDirectory: workingDirectory.FullName);
    }

    public static Task RunDnx(
        string toolName,
        string[] arguments,
        DirectoryInfo workingDirectory,
        string[]? secrets = null)
    {
        return RunAsync("dnx", [toolName, "-y", .. arguments], secrets: secrets ?? [], workingDirectory: workingDirectory.FullName);
    }

    public static Task Dotnet(
        string command,
        string[] arguments,
        DirectoryInfo workingDirectory)
    {
        return RunAsync(
            "dotnet",
            [command, .. arguments],
            workingDirectory: workingDirectory.FullName);
    }
}

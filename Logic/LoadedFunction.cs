namespace SixSlime.AxBind3.Logic;

using System.Diagnostics;

public record LoadedFunction(string Name) : ILoaded
{
    public required string BinaryPath { get; init; }
    public required string[]? Args { get; init; }
    public required string? Stdin { get; init; }
    public required string Proxy { get; init; }

    public async Task<string> Run(string input)
    {
        ProcessStartInfo startInfo = new()
        {
            // Why the fuck not.
            FileName = BinaryPath.Replace(Proxy, input),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            Arguments = Args is not null ? string.Join(' ', Args.Select(x => x.Replace(Proxy, input))) : "",
        };
        var process = Process.Start(startInfo);
        if (process is null)
            throw new ProgramException($"Could not run command '${BinaryPath} {startInfo.Arguments}'");
        var writer = process.StandardInput;
        if (Stdin is not null)
            writer.Write(Stdin.Replace(Proxy, input));
        writer.Close();
        var o = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();
        return o;
    }
}
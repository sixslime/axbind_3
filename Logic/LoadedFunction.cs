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
        // Why the fuck not.
        var binary = BinaryPath.Replace(Proxy, input);
        var args = Args is not null ? string.Join(' ', Args.Select(x => x.Replace(Proxy, input))) : "";
        var stdin = Stdin?.Replace(Proxy, input);
        ProcessStartInfo startInfo = new()
        {
            FileName = binary,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            Arguments = args,
        };
        var process = Process.Start(startInfo);
        if (process is null)
            throw new ProgramException($"Could not run command '${BinaryPath} {startInfo.Arguments}'");
        var writer = process.StandardInput;
        if (stdin is not null)
            writer.Write(stdin);
        writer.Close();
        Logger.VerboseInfo($"    $ {stdin} > {binary} {args}");
        var o = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();
        return o;
    }
}
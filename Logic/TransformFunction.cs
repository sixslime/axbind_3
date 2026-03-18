namespace SixSlime.AxBind3.Logic;

using System.Diagnostics;

internal class TransformFunction(string path)
{
    private readonly string _path = path;

    public async Task<string> Run(string input)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = _path,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        var process = Process.Start(startInfo);
        if (process is null)
            throw new ProgramException($"could not start function '${_path}'");
        var writer = process.StandardInput;
        writer.Write(input);
        writer.Close();
        var o = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();
        return o;
    }
}
namespace SixSlime.AxBind3;

using TomlModel;
using Tomlyn;
using Logic;
using System.CommandLine;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var targetDirArg = new Argument<DirectoryInfo>("target-dir");
        var configDirOption = new Option<DirectoryInfo?>("--config-dir", "-c")
        {
            DefaultValueFactory = _ => Environment.GetEnvironmentVariable("HOME") is { } home ? new(Path.Join(home, ".config", "axbind")) : null
        };
        var profileOption = new Option<string?>("--profile", "-p")
        {
            DefaultValueFactory = _ => "default"
        };
        var rootCommand = new RootCommand("AxBind3")
        {
            targetDirArg,
            profileOption,
            configDirOption
        };
        rootCommand.SetAction(async c =>
        {
            var targetDir = c.GetValue(targetDirArg)!;
            if (!targetDir.Exists)
                throw new ProgramException($"target directory '{targetDir}' does not exist.");
            var configDir = c.GetValue(configDirOption)!;
            if (configDir.Exists)
                throw new ProgramException($"config directory '{configDir}' does not exist");
            var profile = c.GetValue(profileOption)!;
            var resources = new ResourceLoader(configDir.FullName);
            var fileWrites = await resources.LoadProfile(profile).Evaluate(targetDir.FullName, resources);
            await Task.WhenAll(fileWrites.Select(
                fileWrite =>
                    File.WriteAllTextAsync(fileWrite.Path, fileWrite.Contents)));
            return 0;
        });
        try
        {
            return await rootCommand.Parse(args).InvokeAsync();
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case ProgramException v:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(v.Message);
                    return 1;
                case TomlException v:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("[TOML PARSING ERROR]");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(v.Message);
                    return 2;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Error.WriteLine("[UNEXPECTED EXCEPTION]");
                    Console.Error.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Error.WriteLine(ex.StackTrace);
                    return 3;
            }
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
namespace SixSlime.AxBind3;

using TomlModel;
using Tomlyn;
using Logic;
using System.CommandLine;
using System.IO;
public class Program
{
    private static async Task<int> Main(string[] args)
    {
        var targetDirArg = new Argument<DirectoryInfo>("target-dir");
        var outputDirArg = new Argument<DirectoryInfo>("output-dir");
        var configDirOption = new Option<DirectoryInfo?>("--config-dir", "-c")
        {
            Description = "Directory to source profiles/maps/functions from (defaults to $HOME/.config/axbind)",
            DefaultValueFactory = _ => Environment.GetEnvironmentVariable("HOME") is { } home ? new(Path.Join(home, ".config", "axbind")) : null
        };
        var profileOption = new Option<string?>("--profile", "-p")
        {
            Description = $"Profile to execute (name of toml file in <config-dir>/{ResourceLoader.PROFILES_DIR} without '.toml' extension)",
            DefaultValueFactory = _ => "default"
        };
        var safeOption = new Option<bool>("--safe", "-s")
        {
            Description = "Prevent overwriting files in the output directory"
        };
        var rootCommand = new RootCommand("A contrived and declarative text-replacement program")
        {
            targetDirArg,
            outputDirArg,
            profileOption,
            configDirOption,
            safeOption,
        };
        rootCommand.SetAction(async c =>
        {
            var targetDir = c.GetValue(targetDirArg)!;
            if (!targetDir.Exists)
                throw new ProgramException($"target directory '{targetDir}' does not exist.");
            var configDir = c.GetValue(configDirOption)!;
            if (!configDir.Exists)
                throw new ProgramException($"config directory '{configDir}' does not exist");
            var outputDir = c.GetValue(outputDirArg)!;
            var profile = c.GetValue(profileOption)!;
            var safeMode = c.GetValue(safeOption);
            var resources = new ResourceLoader(configDir.FullName);
            var targetTransforms = await resources.LoadProfile(profile).Evaluate(targetDir.FullName, resources);
            await Task.WhenAll(targetTransforms.Select(
                transformed =>
                {
                    var relativePath = Path.GetRelativePath(targetDir.FullName, transformed.Path);
                    var outPath = Path.Join(outputDir.FullName, relativePath);
                    if (safeMode && File.Exists(outPath)) return Task.CompletedTask;
                    if (Path.GetDirectoryName(outPath) is { } containingDir) Directory.CreateDirectory(containingDir);
                    return File.WriteAllTextAsync(outPath, transformed.Contents);
                }));
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
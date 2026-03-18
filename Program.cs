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
        var verboseOption = new Option<bool>("--verbose", "-v")
        {
            Description = "Run with verbose stderr output"
        };
        var quietOption = new Option<bool>("--quiet", "-q")
        {
            Description = "Run with no stderr output (takes precedence over -v)"
        };
        var rootCommand = new RootCommand("A contrived and declarative text-replacement program")
        {
            targetDirArg,
            outputDirArg,
            profileOption,
            configDirOption,
            safeOption,
            verboseOption,
            quietOption,
        };
        rootCommand.SetAction(async c =>
        {
            Logger.SetVerbosity(1);
            if (c.GetValue(verboseOption)) Logger.SetVerbosity(2);
            if (c.GetValue(quietOption)) Logger.SetVerbosity(0);
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
                    Logger.Error("[!] " + v.Message);
                    return 1;
                case TomlException v:
                    Logger.Error("[toml error] " + v.Message);
                    return 2;
                default:
                    Logger.UnexpectedException(ex);
                    return 3;
            }
        }
    }
}
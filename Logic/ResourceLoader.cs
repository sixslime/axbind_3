namespace SixSlime.AxBind3.Logic;

using TomlModel;
using System.Diagnostics;
using System.IO;

// this could be standardized, I dont care.
public class ResourceLoader(string rootConfigPath)
{
    public const string FUNCTIONS_DIR = "functions";
    public const string MAPS_DIR = "maps";
    public const string PROFILES_DIR = "profiles";
    public string RootConfigPath { get; } = rootConfigPath;
    private readonly Dictionary<string, MapFile> _loadedMaps = [];
    private readonly Dictionary<string, ProfileFile> _loadedConfigs = [];
    private readonly Dictionary<string, TransformFunction> _loadedFunctions = [];

    public TransformFunction LoadFunction(string name)
    {
        if (_loadedFunctions.TryGetValue(name, out var cachedVal)) return cachedVal;
        var filePath = Path.Join(RootConfigPath, FUNCTIONS_DIR, name);
        if (!File.Exists(filePath))
            throw new ProgramException($"expected a file at location ${filePath} for map named '${name}'");
        var function = new TransformFunction(filePath);
        _loadedFunctions[name] = function;
        return function;
    }

    public MapFile LoadMap(string name)
    {
        if (_loadedMaps.TryGetValue(name, out var cachedVal)) return cachedVal;
        var filePath = Path.Join(RootConfigPath, MAPS_DIR, name) + ".toml";
        if (!File.Exists(filePath))
            throw new ProgramException($"expected a file at location ${filePath} for map named '${name}'");
        var fileText = File.ReadAllText(filePath);
        var map = MapFile.FromToml(fileText);
        map.ValidateRequiredKeys($"file '{filePath}'");
        _loadedMaps[name] = map;
        return map;
    }

    public ProfileFile LoadProfile(string name)
    {
        if (_loadedConfigs.TryGetValue(name, out var cachedVal)) return cachedVal;
        var filePath = Path.Join(RootConfigPath, PROFILES_DIR, name) + ".toml";
        if (!File.Exists(filePath))
            throw new ProgramException($"expected a file at location ${filePath} for config named '${name}'");
        var fileText = File.ReadAllText(filePath);
        var config = ProfileFile.FromToml(fileText);
        config.ValidateRequiredKeys($"file '{filePath}'");
        _loadedConfigs[name] = config;
        return config;
    }
}
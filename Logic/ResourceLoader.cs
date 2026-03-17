namespace SixSlime.AxBind3.Logic;

using TomlModel;
using System.Diagnostics;
using System.IO;

// this could be standardized, I dont care.
internal class ResourceLoader(string rootConfigPath)
{
    public const string CONFIGS_DIR = "configs";
    public const string FUNCTIONS_DIR = "functions";
    public const string MAPS_DIR = "maps";
    public string RootConfigPath { get; } = rootConfigPath;
    private readonly Dictionary<string, ConfigFile> _loadedConfigs = [];
    private readonly Dictionary<string, MapFile> _loadedMaps = [];
    private readonly Dictionary<string, TransformFunction> _loadedFunctions = [];

    public ConfigFile LoadConfig(string name)
    {
        if (_loadedConfigs.TryGetValue(name, out var cachedVal)) return cachedVal;
        var filePath = Path.Join(RootConfigPath, CONFIGS_DIR, name) + ".toml";
        if (!File.Exists(filePath))
            throw new ProgramException($"expected a file at location ${filePath} for config named '${name}'");
        var fileText = File.ReadAllText(filePath);
        var config = ConfigFile.FromToml(fileText);
        config.ValidateRequiredKeys($"file '{filePath}'");
        _loadedConfigs[name] = config;
        return config;
    }

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
}
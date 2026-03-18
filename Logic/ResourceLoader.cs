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
    private readonly Dictionary<string, LoadedMap> _loadedMaps = [];
    private readonly Dictionary<string, LoadedProfile> _loadedConfigs = [];
    private readonly Dictionary<string, LoadedFunction> _loadedFunctions = [];

    public LoadedFunction LoadFunction(string name)
    {
        if (_loadedFunctions.TryGetValue(name, out var cachedVal)) return cachedVal;
        var fileText = GetResourceContent(name, FUNCTIONS_DIR, "function", out var filePath);
        var model = FunctionFile.FromToml(fileText);
        model.ValidateRequiredKeys("function " + name);
        var loaded = new LoadedFunction(name)
        {
            BinaryPath = model.Function!.Binary!,
            Args = model.Function.Args!.ToArray(),
            Stdin = model.Function.Stdin!,
            Proxy = model.Meta!.Proxy!,
        };
        _loadedFunctions[name] = loaded;
        return loaded;
    }

    public LoadedMap LoadMap(string name)
    {
        if (_loadedMaps.TryGetValue(name, out var cachedVal)) return cachedVal;
        var fileText = GetResourceContent(name, MAPS_DIR, "map", out var filePath);
        var model = MapFile.FromToml(fileText);
        model.ValidateRequiredKeys("map " + name);
        // will happily stack overflow if map inherits itself.
        var loaded = new LoadedMap(name)
        {
            Mappings =
                (model.Meta is null)
                    ? model.Map
                    : model.Meta.Inherit!
                        .Select(inherited => LoadMap(inherited).Mappings)
                        .SelectMany(x => x)
                        .Concat(model.Map)
                        .GroupBy(x => x.Key)
                        .ToDictionary(group => group.Key, group => group.Last().Value)
        };
        _loadedMaps[name] = loaded;
        return loaded;
    }

    public LoadedProfile LoadProfile(string name)
    {
        if (_loadedConfigs.TryGetValue(name, out var cachedVal)) return cachedVal;
        var filePath = Path.Join(RootConfigPath, PROFILES_DIR, name) + ".toml";
        if (!File.Exists(filePath))
            throw new ProgramException($"expected a file at location ${filePath} for config named '${name}'");
        var fileText = File.ReadAllText(filePath);
        var model = ProfileFile.FromToml(fileText);
        model.ValidateRequiredKeys("profile " + name);
        var loaded = new LoadedProfile(name)
        {
            Passes = model.Passes.Select(
                passModel =>
                    new LoadedProfile.Pass()
                    {
                        CaptureStart = passModel.CaptureStart!,
                        CaptureEnd = passModel.CaptureEnd!,
                        CaptureEscapeSequence = passModel.CaptureEscapeSequence,
                        Files = passModel.Files!,
                        Layers = passModel.Layers.Select(
                            layerModel =>
                                new LoadedProfile.Layer()
                                {
                                    Files = layerModel.Files,
                                    Transform = layerModel.Transform! switch
                                    {
                                        var v when v.Map is not null => new LoadedProfile.ELayerTransform.Map()
                                        {
                                            Name = v.Map
                                        },
                                        var v when v.Function is not null => new LoadedProfile.ELayerTransform.Function()
                                        {
                                            Name = v.Function
                                        },
                                        _ => throw new NotSupportedException()
                                    }
                                }).ToList(),
                    }).ToList()
        };
        _loadedConfigs[name] = loaded;
        return loaded;
    }

    private string GetResourceContent(string name, string dir, string humanTypeName, out string filePath)
    {
        filePath = Path.Join(RootConfigPath, dir, name) + ".toml";
        if (!File.Exists(filePath))
            throw new ProgramException($"expected a file at location ${filePath} for {humanTypeName} named '${name}'");
        return File.ReadAllText(filePath);
    }
}
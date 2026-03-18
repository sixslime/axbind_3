namespace SixSlime.AxBind3.TomlModel;
using Tomlyn.Serialization;
using Tomlyn;
using Logic;
public class MapFile : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys =>
    [
        (Map, "map")
    ];
    public MapMetaOptions? Meta { get; set; }
    public Dictionary<string, string> Map { get; set; } = [];

    // will happily stack overflow if map inherits itself.
    public Dictionary<string, string> GetMappings(ResourceLoader loader)
    {
        if (Meta is null) return Map;
        if (Meta.Inherit is null || Meta.Inherit.Count == 0) return Map;
        return Meta.Inherit
            .Select(name => loader.LoadMap(name).GetMappings(loader))
            .SelectMany(x => x)
            .Concat(Map)
            .GroupBy(x => x.Key)
            .ToDictionary(group => group.Key, group => group.Last().Value);
    }
    public static MapFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.MapFile)!;
    }
}

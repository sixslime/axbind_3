namespace SixSlime.AxBind3.TomlModel;
using Tomlyn.Serialization;
using Tomlyn;

public class MapFile : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys =>
    [
        (Map, "map")
    ];
    public MapMetaOptions? Meta { get; set; }
    public Dictionary<string, string> Map { get; set; } = [];

    public static MapFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.MapFile)!;
    }
}

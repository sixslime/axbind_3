namespace SixSlime.AxBind3.TomlModel;

using Tomlyn.Serialization;
using Tomlyn;
using Logic;

public class MapFile : TomlValidatable
{
    public static MapFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.MapFile)!;
    }

    public Dictionary<string, string> Map { get; set; } = [];
    public MapMetaOptions? Meta { get; set; }

    protected override (object?, string)[] RequiredKeys =>
    [
        (Map, "map")
    ];

}
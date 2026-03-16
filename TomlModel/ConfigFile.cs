namespace SixSlime.AxBind3.TomlModel;
using Tomlyn.Serialization;
using Tomlyn;


public class ConfigFile : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys => [];
    public List<Pass> Passes { get; set; } = [];

    public static ConfigFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.ConfigFile)!;
    }
}

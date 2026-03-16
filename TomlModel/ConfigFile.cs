namespace SixSlime.AxBind3.Types;
using Tomlyn.Serialization;
using Tomlyn;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ConfigFile
{
    public List<Pass> Passes { get; set; } = [];

    public static ConfigFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.ConfigFile)!;
    }
}
[TomlSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[TomlSerializable(typeof(ConfigFile))]
internal partial class AppTomlContext : TomlSerializerContext;

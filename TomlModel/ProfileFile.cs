namespace SixSlime.AxBind3.TomlModel;

using Tomlyn.Serialization;
using Tomlyn;
using Logic;

public class ProfileFile : TomlValidatable
{
    public static ProfileFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.ProfileFile)!;
    }

    public List<Pass> Passes { get; set; } = [];

    protected override (object?, string, bool)[] CheckedKeys =>
    [
        (Passes, "pass", true)
    ];

}
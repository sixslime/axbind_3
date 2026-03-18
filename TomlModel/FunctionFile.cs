namespace SixSlime.AxBind3.TomlModel;

using Tomlyn.Serialization;
using Tomlyn;
using Logic;

public class FunctionFile : TomlValidatable
{
    public static FunctionFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.FunctionFile)!;
    }

    public FunctionInfo? Function { get; set; }
    public FunctionMetaOptions? Meta { get; set; }

    protected override (object?, string)[] RequiredKeys =>
    [
        (Meta, "meta"),
        (Function, "function"),
    ];
}
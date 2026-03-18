namespace SixSlime.AxBind3.TomlModel;

public class FunctionMetaOptions : TomlValidatable
{
    public string? Proxy { get; set; }
    protected override (object?, string)[] RequiredKeys => [(Proxy, "proxy")];
}
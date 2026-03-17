namespace SixSlime.AxBind3.TomlModel;

public class MapMetaOptions : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys => [];
    public List<string>? Inherit { get; set; } = [];
}
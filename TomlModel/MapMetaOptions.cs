namespace SixSlime.AxBind3.TomlModel;

public class MapMetaOptions : TomlValidatable
{
    public List<string>? Inherit { get; set; } = [];
    protected override (object?, string, bool)[] CheckedKeys => [];
}
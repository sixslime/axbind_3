namespace SixSlime.AxBind3.Types;

public class LayerTransform : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys => [];
    public string? Map { get; set; }
    public string? Function { get; set; }
}
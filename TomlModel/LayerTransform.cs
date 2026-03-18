namespace SixSlime.AxBind3.TomlModel;

public class LayerTransform : TomlValidatable
{
    public string? Function { get; set; }
    public string? Map { get; set; }
    protected override (object?, string, bool)[] CheckedKeys => [];
}
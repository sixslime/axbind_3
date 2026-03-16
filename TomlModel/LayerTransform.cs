namespace SixSlime.AxBind3.TomlModel;

public class LayerTransform : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys => [];
    public string? Map { get; set; }
    public string? Function { get; set; }
}
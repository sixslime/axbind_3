namespace SixSlime.AxBind3.TomlModel;

public class Layer : TomlValidatable
{
    public LayerTransform? Transform { get; set; }

    public List<string>? Files { get; set; }

    protected override (object?, string, bool)[] CheckedKeys =>
    [
        (Transform, "transform", true)
    ];
}
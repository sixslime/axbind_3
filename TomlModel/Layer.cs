namespace SixSlime.AxBind3.TomlModel;

public class Layer : TomlValidatable
{
    public LayerTransform? Transform { get; set; }

    public string? Files { get; set; }

    protected override (object?, string)[] RequiredKeys =>
    [
        (Transform, "transform")
    ];
}
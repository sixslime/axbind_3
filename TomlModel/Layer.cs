namespace SixSlime.AxBind3.TomlModel;

public class Layer : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys =>
    [
        (Transform, "transform")
    ];

    public string? Files { get; set; }
    public LayerTransform? Transform { get; set; }
}
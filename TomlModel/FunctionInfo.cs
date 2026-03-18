namespace SixSlime.AxBind3.TomlModel;

public class FunctionInfo : TomlValidatable
{
    public string? Binary { get; set; }
    public string? Stdin { get; set; }
    public List<string>? Args { get; set; }

    protected override (object?, string, bool)[] CheckedKeys =>
    [
        (Binary, "binary", true),
    ];
}
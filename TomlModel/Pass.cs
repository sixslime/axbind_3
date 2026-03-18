namespace SixSlime.AxBind3.TomlModel;

public class Pass : TomlValidatable
{
    public List<Layer> Layers { get; set; } = [];
    public string? CaptureEnd { get; set; }
    public string? CaptureEscapeSequence { get; set; }
    public string? CaptureStart { get; set; }
    public List<string>? Files { get; set; }

    protected override (object?, string, bool)[] CheckedKeys =>
    [
        (CaptureStart, "capture_start", true),
        (CaptureEnd, "capture_end", true),
        (Files, "files", true),
        (Layers, "layers", true),
    ];
}
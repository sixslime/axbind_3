namespace SixSlime.AxBind3.TomlModel;

public class Pass : TomlValidatable
{
    public List<Layer> Layers { get; set; } = [];
    public string? CaptureEnd { get; set; }
    public string? CaptureEscapeSequence { get; set; }
    public string? CaptureStart { get; set; }
    public List<string>? Files { get; set; }

    protected override (object?, string)[] RequiredKeys =>
    [
        (CaptureStart, "capture_start"),
        (CaptureEnd, "capture_end"),
        (Files, "files")
    ];
}
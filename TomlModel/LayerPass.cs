namespace SixSlime.AxBind3.Types;

public class Pass : TomlValidatable
{
    protected override (object?, string)[] RequiredKeys =>
    [
        (CaptureStart, "capture_start"),
        (CaptureEnd, "capture_end"),
        (Files, "files"),
    ];
    public string? CaptureStart { get; set; }
    public string? CaptureEnd { get; set; }
    public string? CaptureEscapeSequence { get; set; }
    public string? Files { get; set; }
    public List<Layer> Layers { get; set; } = [];
}
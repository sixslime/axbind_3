namespace SixSlime.AxBind3.Logic;

internal record TargetFile
{
    public required string OriginalText { get; init; }
    public required string Path { get; init; }
}
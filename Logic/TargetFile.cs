namespace SixSlime.AxBind3.Logic;

public sealed record TargetFile
{
    public required string Path { get; init; }
    public required string Contents { get; init; }
}
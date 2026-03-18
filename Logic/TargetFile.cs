namespace SixSlime.AxBind3.Logic;

public sealed record TargetFile
{
    public required string Contents { get; init; }
    public required string Path { get; init; }
}
namespace SixSlime.AxBind3.Logic;

public record LoadedMap(string Name) : ILoaded
{
    public required IReadOnlyDictionary<string, string> Mappings { get; init; }
}
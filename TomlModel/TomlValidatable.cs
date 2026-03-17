namespace SixSlime.AxBind3.TomlModel;
using Tomlyn.Serialization;
public abstract class TomlValidatable
{
    [TomlIgnore]
    protected abstract (object?, string)[] RequiredKeys { get; }
    public void ValidateRequiredKeys(string contextLocation)
    {
        var missing = RequiredKeys.Where(x => x.Item1 is null).Select(x => x.Item2).ToArray();
        if (missing.Length > 0)
            throw new ProgramException($"required keys [{string.Join(", ", missing)}] are not present in: {contextLocation}");
    }
}
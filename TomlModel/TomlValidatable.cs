namespace SixSlime.AxBind3.TomlModel;

public abstract class TomlValidatable
{
    protected abstract (object?, string)[] RequiredKeys { get; }
    private RequiredKeysMissingException? ValidateRequiredKeys(params (object?, string)[] requiredKeys)
    {
        var missing = requiredKeys.Where(x => x.Item1 is null).Select(x => x.Item2).ToArray();
        return missing.Length > 0 ? new RequiredKeysMissingException(missing) : null;
    }
}
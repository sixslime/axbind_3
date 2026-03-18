namespace SixSlime.AxBind3.TomlModel;

using Tomlyn.Serialization;

public abstract class TomlValidatable
{
    [TomlIgnore] protected abstract (object?, string)[] RequiredKeys { get; }

    public void ValidateRequiredKeys(string contextLocation, bool recursive = true)
    {
        List<string> missing = [];
        List<KeyValuePair<string, TomlValidatable>> toValidate = [];
        List<KeyValuePair<string, IEnumerable<TomlValidatable>>> toValidateEach = [];
        foreach (var (obj, name) in RequiredKeys)
        {
            if (obj is null) missing.Add(name);
            if (recursive && obj is TomlValidatable val) toValidate.Add(KeyValuePair.Create(name, val));
            if (recursive && obj is IEnumerable<TomlValidatable> valEach) toValidateEach.Add(KeyValuePair.Create(name, valEach));
        }
        if (missing.Count > 0)
            throw new ProgramException($"required keys [{string.Join(", ", missing)}] are not present in: {contextLocation}");
        foreach (var (name, obj) in toValidate)
            obj.ValidateRequiredKeys(contextLocation + "." + name);

        foreach (var (name, list) in toValidateEach)
        {
            int i = 0;
            foreach (var obj in list)
                obj.ValidateRequiredKeys($"{contextLocation}.{name}[{i}]");
        }
    }
}
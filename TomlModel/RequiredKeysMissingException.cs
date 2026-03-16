namespace SixSlime.AxBind3.TomlModel;

public class RequiredKeysMissingException(string[] missingKeys)
{
    public string[] MissingKeys { get; } = missingKeys;
   
}
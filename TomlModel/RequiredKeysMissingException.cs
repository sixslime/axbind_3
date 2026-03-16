namespace SixSlime.AxBind3.Types;

public class RequiredKeysMissingException(string[] missingKeys)
{
    public string[] MissingKeys { get; } = missingKeys;
   
}
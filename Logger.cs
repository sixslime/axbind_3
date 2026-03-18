namespace SixSlime.AxBind3;

public static class Logger
{
    private static int? _verbosity = null;

    public static void SetVerbosity(int verbosity)
    {
        _verbosity = verbosity;
    }

    public static void VerboseInfo(string message)
    {
        if (GetVerbosity() < 2) return;
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Error.WriteLine($"~ {message}");
        Console.ResetColor();
    }

    public static void Info(string message)
    {
        if (GetVerbosity() < 1) return;
        Console.Error.WriteLine($"> {message}");
    }

    public static void Error(string message)
    {
        if (GetVerbosity() < 1) return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public static void UnexpectedException(Exception ex)
    {
        if (GetVerbosity() < 1) return;
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Error.WriteLine("[UNEXPECTED EXCEPTION!!!]");
        Console.Error.WriteLine(ex);
        Console.ResetColor();
    }

    private static int GetVerbosity()
    {
        if (_verbosity is null) throw new Exception("Logger used without initialization?");
        return (int)_verbosity;
    }
}
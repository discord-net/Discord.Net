namespace Discord.Net.Hanz;

public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    None = 5
}

public interface ILogger
{
    bool IsEnabled(LogLevel logLevel);
    void Log(LogLevel logLevel, string message);
}

// does what it supposed to do - nothing
public sealed class NullLogger : ILogger
{
    public static readonly ILogger Instance = new NullLogger();

    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log(LogLevel logLevel, string message) { }
}

public static class LoggerExtensions
{
    public static void Log(this ILogger logger, string message) => logger.Log(LogLevel.Information, message);
    public static void Warn(this ILogger logger, string message) => logger.Log(LogLevel.Warning, message);

}

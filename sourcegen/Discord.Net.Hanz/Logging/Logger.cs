namespace Discord.Net.Hanz;

public sealed class Logger : ILogger
{
    private readonly LogLevel _logLevel;
    private readonly string _logFilePath;

    public Logger(
        LogLevel logLevel,
        string logFilePath)
    {
        _logLevel = logLevel;
        _logFilePath = logFilePath;

        if(File.Exists(_logFilePath))
            File.Delete(_logFilePath);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _logLevel;
    }

    public void Log(LogLevel logLevel, string message)
    {
        if (!IsEnabled(logLevel))
            return;

        try
        {
            File.AppendAllText(_logFilePath,
                $"[{DateTime.Now:O} | {logLevel}] {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            SelfLog.Write(ex.ToString());
        }
    }
}

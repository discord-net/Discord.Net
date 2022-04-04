private static async Task LogAsync(LogMessage message)
{
    var severity = message.Severity switch
    {
        LogSeverity.Critical => LogEventLevel.Fatal,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Info => LogEventLevel.Information,
        LogSeverity.Verbose => LogEventLevel.Debug,
        LogSeverity.Debug => LogEventLevel.Verbose,
        _ => LogEventLevel.Information
    };
    Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
    await Task.CompletedTask;
}

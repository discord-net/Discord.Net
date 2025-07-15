namespace Discord.Net.Hanz;

public record LogContext(
    Type Owner,
    params object[] Details
);

file sealed class LoggerProxy : ILogger
{
    public LogContext Context { get; }

    public LoggerProxy(LogContext context)
    {
        Context = context;
    }

    public void Dispose() {}

    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log(LogLevel logLevel, string message)
    {
    }

    public void Clean()
    {
    }

    public void Flush()
    {
    }
}

public static class Logging
{
    public static bool IsInitialized => _loggerFactory is not null;
    
    private static Func<LogContext, ILogger>? _loggerFactory = null;

    private static readonly HashSet<ILogger> _loggers = [];

    private static readonly object _lock = new();

    public static void Reset()
    {
        // lock (_lock)
        // {
        //     foreach (var logger in _loggers)
        //     {
        //         logger.Dispose();
        //     }
        //
        //     _loggers.Clear();
        // }
    }

    public static void InitializeFileLogging(
        string path,
        LogLevel level
    )
    {
        lock (_lock)
        {
            if (_loggerFactory is not null)
            {
                foreach (var logger in _loggers)
                {
                    logger.Dispose();
                }

                _loggers.Clear();
                
                return;
            }

            var fileLogger = new FileLogger(
                Path.Combine(path, "FileLogging.log"),
                LogLevel.Trace,
                new(typeof(Logging))
            );
            
            _loggerFactory = (ctx) =>
            {
                var detailsPath = ctx.Details.Select(x => x.ToString());

                var filePath = Path.Combine([path, ..detailsPath, $"{ctx.Owner.Name}.log"]);

                if (_loggers.OfType<FileLogger>().FirstOrDefault(x => x.Path == filePath) is { } logger)
                {
                    fileLogger.Log($"CACHE: {filePath} : {ctx}");
                    return logger;
                }

                if (FileLogger.TryCreate(filePath, level, ctx, out logger))
                {
                    fileLogger.Log($"CREATE({_loggers.Add(logger)}): {filePath} : {ctx}");
                    return logger;
                }
                
                fileLogger.Log($"NULL: {filePath} : {ctx}");
                return NullLogger.Instance;
            };
            
            UpdateProxies();
        }

        
    }

    private static void UpdateProxies()
    {
        if(_loggerFactory is null) return;
        
        foreach (var proxy in _loggers.OfType<LoggerProxy>().ToArray())
        {
            _loggers.Remove(proxy);
            _loggers.Add(_loggerFactory(proxy.Context));
        }
    }

    public static ILogger GetLogger<T>()
        => GetLogger(new(typeof(T)));

    public static ILogger GetLogger<T>(params object[] details)
        => GetLogger(new(typeof(T), details));

    public static ILogger GetLogger(LogContext context)
    {
        lock (_lock)
        {
            var logger = _loggerFactory is null
                ? new LoggerProxy(context)
                : _loggerFactory(context);
            
            _loggers.Add(logger);

            return logger;
        }
    }
}
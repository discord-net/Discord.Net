using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;

namespace Discord.Net.Hanz;

public sealed class Logger : ILogger, IEquatable<Logger>
{
    public const int MAX_UNFLUSHED_LOGS = 2500;

    public static readonly string LogDirectory = Path.Combine(Environment.CurrentDirectory, ".hanz");
    private readonly ConcurrentDictionary<string, Logger> _subLoggers = [];

    private readonly LogLevel _logLevel;
    private readonly string _logFilePath;

    private readonly LinkedList<string> _logs;

    private readonly object _syncRoot = new();

    public Logger(
        LogLevel logLevel,
        string logFilePath)
    {
        _logs = new();
        _logLevel = logLevel;
        _logFilePath = logFilePath;

        var dir = Path.GetDirectoryName(logFilePath);
        if (dir is not null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public Logger GetSubLogger(string name)
        => _subLoggers.GetOrAdd(
            name,
            name =>
            {
                var logger = new Logger(
                    _logLevel,
                    Path.Combine(
                        Path.GetDirectoryName(_logFilePath) ?? LogDirectory,
                        Path.GetFileNameWithoutExtension(_logFilePath),
                        $"{name}.subtask.log"
                    )
                );
                logger.DeleteLogFile();
                return logger;
            }
        );

    public Logger WithCleanLogFile()
    {
        DeleteLogFile();
        return this;
    }

    public void Clean()
    {
        DeleteLogFile();
        foreach (var subLogger in _subLoggers.ToArray())
        {
            subLogger.Value.Clean();
        }
    }

    public Logger WithSemanticContext(SemanticModel model)
    {
        if (_logFilePath.Contains(model.Compilation.Assembly.Name))
            return this;

        var name = Path.Combine(LogDirectory, model.Compilation.Assembly.Name,
            Path.GetFileName(_logFilePath));

        return _subLoggers.GetOrAdd(name, name =>
        {
            var logger = new Logger(_logLevel, name);
            logger.DeleteLogFile();
            return logger;
        });
    }

    public void DeleteLogFile()
    {
        if (_logFilePath.ToLowerInvariant().Contains("roslyn"))
            return;

        lock (_syncRoot)
        {
            if (File.Exists(_logFilePath))
                File.Delete(_logFilePath);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _logLevel;
    }

    public void Log(LogLevel logLevel, string message)
    {
        if (!IsEnabled(logLevel))
            return;

        if (_logFilePath.ToLowerInvariant().Contains("roslyn"))
            return;

        try
        {
            lock (_syncRoot)
            {
                _logs.AddLast($"[{DateTime.Now:O} | {logLevel}] {message}");

                if(_logs.Count >= MAX_UNFLUSHED_LOGS)
                    NoLockFlush();
            }
        }
        catch (Exception ex)
        {
            SelfLog.Write(ex.ToString());
        }
    }

    public void Flush()
    {
        lock (_syncRoot)
        {
            NoLockFlush();
        }
    }

    private void NoLockFlush()
    {
        if (_logs.Count >= 0)
        {
            using (var writer = File.AppendText(_logFilePath))
            {
                foreach (var log in _logs)
                    writer.WriteLine(log);

                writer.Flush();
            }

            _logs.Clear();
        }

        foreach (var subLogger in _subLoggers)
            subLogger.Value.Flush();
    }

    public static Logger CreateSemanticRun(string assembly)
    {
        return new Logger(Hanz.LoggerOptions.Level,
            Path.Combine(LogDirectory, assembly, "latest.log"));
    }

    public static Logger CreateSemanticRunForTask(string assembly, string task)
    {
        return new Logger(Hanz.LoggerOptions.Level,
            Path.Combine(LogDirectory, assembly, $"{task}.task.log"));
    }

    public static Logger CreateForTask(string task)
    {
        return new Logger(Hanz.LoggerOptions.Level,
            Path.Combine(LogDirectory, $"{task}.task.log"));
    }

    public bool Equals(Logger? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _logLevel == other._logLevel && _logFilePath == other._logFilePath;
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Logger other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)_logLevel * 397) ^ _logFilePath.GetHashCode();
        }
    }

    public static bool operator ==(Logger? left, Logger? right) => Equals(left, right);

    public static bool operator !=(Logger? left, Logger? right) => !Equals(left, right);
}

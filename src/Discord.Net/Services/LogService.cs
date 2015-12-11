using System;

namespace Discord
{
    public class LogService : IService
    {
		public DiscordClient Client => _client;
		private DiscordClient _client;

		public LogSeverity Level => _level;
		private LogSeverity _level;

		public event EventHandler<LogMessageEventArgs> LogMessage;
		internal void RaiseLogMessage(LogMessageEventArgs e)
		{
			if (LogMessage != null)
			{
				try
				{
					LogMessage(this, e);
				}
				catch { } //We dont want to log on log errors
			}
		}

		void IService.Install(DiscordClient client)
		{
			_client = client;
			_level = client.Config.LogLevel;
		}

		public Logger CreateLogger(string source)
		{
			return new Logger(this, source);
		}
	}

	public class Logger
	{
		private LogService _service;

		public LogSeverity Level => _level;
		private LogSeverity _level;

		public string Source => _source;
		private string _source;

		internal Logger(LogService service, string source)
		{
			_service = service;
			_level = service.Level;
			_source = source;
		}

		public void Log(LogSeverity severity, string message, Exception exception = null)
		{
			if (severity <= _service.Level)
				_service.RaiseLogMessage(new LogMessageEventArgs(severity, _source, message, exception));
        }
        public void Error(string message, Exception exception = null)
            => Log(LogSeverity.Error, message, exception);
        public void Warning(string message, Exception exception = null)
            => Log(LogSeverity.Warning, message, exception);
        public void Info(string message, Exception exception = null)
            => Log(LogSeverity.Info, message, exception);
        public void Verbose(string message, Exception exception = null)
            => Log(LogSeverity.Verbose, message, exception);
        public void Debug(string message, Exception exception = null)
            => Log(LogSeverity.Debug, message, exception);
    }
}

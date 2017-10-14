using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Discord.MicrosoftLogging
{
    public class LogAdapter
    {
        private readonly ILogger _logger;
        private readonly Func<LogMessage, Exception, string> _formatter;

        /// <summary>
        /// Creates a LogAdapter to be used with a Discord client
        /// </summary>
        /// <param name="logger">The logger implementation that messages will be written to</param>
        /// <param name="formatter">
        /// A custom message formatter, should the default be inadequate.
        /// 
        /// The default message formatter simply returns <see cref="LogMessage.ToString()"/>, which 
        /// in most cases should not be a problem.
        /// </param>
        public LogAdapter(ILogger logger, Func<LogMessage, Exception, string> formatter = null)
        {
            _logger = logger;
            _formatter = formatter ?? DefaultFormatter;
        }

        /// <summary>
        /// Convert a Discord.Net log event to an abstract log event
        /// </summary>
        /// <param name="message">The log event to be converted</param>
        /// <returns>A task for compatibility with Discord.Net's async events</returns>
        public Task Log(LogMessage message)
        {
            _logger.Log(GetLogLevel(message.Severity), default(EventId), message, message.Exception, _formatter);
            return Task.Delay(0);
        }

        private string DefaultFormatter(LogMessage message, Exception _)
            => message.ToString();
        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel)(Math.Abs((int)severity - 5));
    }
}

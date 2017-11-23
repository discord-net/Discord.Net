using System;
using Microsoft.Extensions.Logging;

namespace Discord.MicrosoftLogging
{
    public static class Extensions
    {
        /// <summary>
        /// Configure this Discord client with support for Microsoft's logging abstractions
        /// </summary>
        /// <param name="client">The Discord client to hook into</param>
        /// <param name="logger">A logger created for logging to</param>
        /// <param name="formatter">
        /// A custom message formatter, should the default one not suffice. 
        /// 
        /// See <see cref="LogAdapter(ILogger, Func{LogMessage, Exception, string})"/> for more information.</param>
        public static void UseMicrosoftLogging(this IDiscordClient client, ILogger logger, Func<LogMessage, Exception, string> formatter = null)
        {
            var adaptor = new LogAdapter(logger, formatter);
            client.Log += adaptor.Log;
        }
    }
}

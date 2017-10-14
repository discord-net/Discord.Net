using System;
using Microsoft.Extensions.Logging;

namespace Discord.MicrosoftLogging
{
    public static class Extensions
    {
        public static void UseMicrosoftLogging(this IDiscordClient client, ILogger logger, Func<LogMessage, Exception, string> formatter = null)
        {
            var adaptor = new LogAdaptor(logger, formatter);
            client.Log += adaptor.Log;
        }
    }
}

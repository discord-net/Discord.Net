using System;
using System.Net;

namespace Discord.Net
{
    public class HttpException : Exception
    {
        public HttpStatusCode HttpCode { get; }
        public int? DiscordCode { get; }
        public string Reason { get; }
        public IRequest Request { get; }

        public HttpException(HttpStatusCode httpCode, IRequest request, int? discordCode = null, string reason = null)
            : base(CreateMessage(httpCode, discordCode, reason))
        {
            HttpCode = httpCode;
            Request = request;
            DiscordCode = discordCode;
            Reason = reason;
        }

        private static string CreateMessage(HttpStatusCode httpCode, int? discordCode = null, string reason = null)
        {   
            string msg;
            if (discordCode != null && discordCode != 0)
            {
                if (reason != null)
                    msg = $"The server responded with error {(int)discordCode}: {reason}";
                else
                    msg = $"The server responded with error {(int)discordCode}: {httpCode}";
            }
            else
            {
                if (reason != null)
                    msg = $"The server responded with error {(int)httpCode}: {reason}";
                else
                    msg = $"The server responded with error {(int)httpCode}: {httpCode}";
            }
            return msg;
        }
    }
}

using System;
using System.Net;

namespace Discord.Net
{
    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode httpCode, IRequest request, int? discordCode = null, string reason = null)
            : base(CreateMessage(httpCode, discordCode, reason))
        {
            HttpCode = httpCode;
            Request = request;
            DiscordCode = discordCode;
            Reason = reason;
        }

        public HttpStatusCode HttpCode { get; }
        public int? DiscordCode { get; }
        public string Reason { get; }
        public IRequest Request { get; }

        private static string CreateMessage(HttpStatusCode httpCode, int? discordCode = null, string reason = null)
        {
            string msg;
            if (discordCode != null && discordCode != 0)
            {
                msg = reason != null ? $"The server responded with error {(int)discordCode}: {reason}" : $"The server responded with error {(int)discordCode}: {httpCode}";
            }
            else
            {
                msg = reason != null ? $"The server responded with error {(int)httpCode}: {reason}" : $"The server responded with error {(int)httpCode}: {httpCode}";
            }

            return msg;
        }
    }
}

using System;
using System.Net;

namespace Discord.Net
{
    /// <summary>
    ///     Describes an exception that occurred during the processing of Discord HTTP requests.
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        ///     Gets the HTTP status code returned by Discord.
        /// </summary>
        public HttpStatusCode HttpCode { get; }
        /// <summary>
        ///     Gets the JSON error code returned by Discord, or <see langword="null"/> if none.
        /// </summary>
        public int? DiscordCode { get; }
        /// <summary>
        ///     Gets the reason of the exception.
        /// </summary>
        public string Reason { get; }
        /// <summary>
        ///     Gets the request object used to send the request.
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
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

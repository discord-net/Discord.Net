using System;
using System.Net;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Represents errors received in response to rest requests.
    /// </summary>
    public class DiscordRestException : Exception
    {
        /// <summary>
        /// HTTP status code returned.
        /// </summary>
        public HttpStatusCode HttpCode { get; }

        /// <summary>
        /// Discord JSON error code.
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#json-json-error-codes"/>
        /// </remarks>
        public int? DiscordCode { get; }

        /// <summary>
        /// Reason of this error.
        /// </summary>
        public string? Reason { get; }

        /// <summary>
        /// Creates a <see cref="DiscordRestException"/> with the provided error data.
        /// </summary>
        /// <param name="httpCode">
        /// HTTP status code.
        /// </param>
        /// <param name="discordCode">
        /// Discord JSON error code.
        /// </param>
        /// <param name="reason">
        /// Reason of this error.
        /// </param>
        public DiscordRestException(HttpStatusCode httpCode, int? discordCode = null, string? reason = null)
            : base(CreateMessage(httpCode, discordCode, reason))
        {
            HttpCode = httpCode;
            DiscordCode = discordCode;
            Reason = reason;
        }

        private static string CreateMessage(HttpStatusCode httpCode, int? discordCode = null, string? reason = null)
        {
            if (!string.IsNullOrEmpty(reason))
                return $"The server responded with error {discordCode ?? (int)httpCode}: {reason}";
            else
                return $"The server responded with error {discordCode ?? (int)httpCode}: {httpCode}";
        }
    }
}

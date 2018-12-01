using System;

namespace Discord
{
    /// <summary>
    ///     Provides a series of helper methods for handling Discord login tokens.
    /// </summary>
    public static class TokenUtils
    {
        /// <summary>
        ///     The minimum length of a Bot token.
        /// </summary>
        /// <remarks>
        ///     This value was determined by comparing against the examples in the Discord
        ///     documentation, and pre-existing tokens.
        /// </remarks>
        internal const int MinBotTokenLength = 58;

        /// <summary>
        ///     Checks the validity of a bot token by attempting to decode a ulong userid
        ///     from the bot token.
        /// </summary>
        /// <param name="message">
        ///     The bot token to validate.
        /// </param>
        /// <returns>
        ///     True if the bot token was valid, false if it was not.
        /// </returns>
        internal static bool CheckBotTokenValidity(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            // split each component of the JWT
            var segments = message.Split('.');

            // ensure that there are three parts
            if (segments.Length != 3)
                return false;

            try
            {
                // decode the first segment as base64
                var v = Convert.FromBase64String(segments[0]);
                BitConverter.ToUInt64(v, 0);
                // if no exception thrown, token is valid
                return true;
            }
            catch (FormatException)
            {
                // ignore exception, if contains invalid base64 characters return false
                return false;
            }
            catch (ArgumentException)
            {
                // ignore exceptions thrown by BitConverter
                return false;
            }
        }

        /// <summary>
        ///     Checks the validity of the supplied token of a specific type.
        /// </summary>
        /// <param name="tokenType"> The type of token to validate. </param>
        /// <param name="token"> The token value to validate. </param>
        /// <exception cref="ArgumentNullException"> Thrown when the supplied token string is <c>null</c>, empty, or contains only whitespace.</exception>
        /// <exception cref="ArgumentException"> Thrown when the supplied <see cref="TokenType"/> or token value is invalid. </exception>
        public static void ValidateToken(TokenType tokenType, string token)
        {
            // A Null or WhiteSpace token of any type is invalid.
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(paramName: nameof(token), message: "A token cannot be null, empty, or contain only whitespace.");

            switch (tokenType)
            {
                case TokenType.Webhook:
                    // no validation is performed on Webhook tokens
                    break;
                case TokenType.Bearer:
                    // no validation is performed on Bearer tokens
                    break;
                case TokenType.Bot:
                    // bot tokens are assumed to be at least 58 characters in length
                    // this value was determined by referencing examples in the discord documentation, and by comparing with
                    // pre-existing tokens
                    if (token.Length < MinBotTokenLength)
                        throw new ArgumentException(message: $"A Bot token must be at least {MinBotTokenLength} characters in length.", paramName: nameof(token));
                    // check the validity of the bot token by decoding the ulong userid from the jwt
                    if (!CheckBotTokenValidity(token))
                        throw new ArgumentException(message: "The Bot token was invalid.", paramName: nameof(token));
                    break;
                default:
                    // All unrecognized TokenTypes (including User tokens) are considered to be invalid.
                    throw new ArgumentException(message: "Unrecognized TokenType.", paramName: nameof(token));
            }
        }
    }
}

using System;
using System.Globalization;
using System.Text;

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

        internal const char Base64Padding = '=';

        /// <summary>
        ///     Pads a base64-encoded string with 0, 1, or 2 '=' characters,
        ///     if the string is not a valid multiple of 4.
        ///     Does not ensure that the provided string contains only valid base64 characters.
        ///     Strings that already contain padding will not have any more padding applied.
        /// </summary>
        /// <remarks>
        ///     A string that would require 3 padding characters is considered to be already corrupt.
        ///     Some older bot tokens may require padding, as the format provided by Discord
        ///     does not include this padding in the token.
        /// </remarks>
        /// <param name="encodedBase64">The base64 encoded string to pad with characters.</param>
        /// <returns>A string containing the base64 padding.</returns>
        /// <exception cref="FormatException">
        ///     Thrown if <paramref name="encodedBase64"/> would require an invalid number of padding characters.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="encodedBase64"/> is null, empty, or whitespace.
        /// </exception>
        internal static string PadBase64String(string encodedBase64)
        {
            if (string.IsNullOrWhiteSpace(encodedBase64))
                throw new ArgumentNullException(paramName: encodedBase64,
                    message: "The supplied base64-encoded string was null or whitespace.");

            // do not pad if already contains padding characters
            if (encodedBase64.IndexOf(Base64Padding) != -1)
                return encodedBase64;

            // based from https://stackoverflow.com/a/1228744
            var padding = (4 - (encodedBase64.Length % 4)) % 4;
            if (padding == 3)
                // can never have 3 characters of padding
                throw new FormatException("The provided base64 string is corrupt, as it requires an invalid amount of padding.");
            else if (padding == 0)
                return encodedBase64;
            return encodedBase64.PadRight(encodedBase64.Length + padding, Base64Padding);
        }

        /// <summary>
        ///     Decodes a base 64 encoded string into a ulong value.
        /// </summary>
        /// <param name="encoded"> A base 64 encoded string containing a User Id.</param>
        /// <returns> A ulong containing the decoded value of the string, or null if the value was invalid. </returns>
        internal static ulong? DecodeBase64UserId(string encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded))
                return null;

            try
            {
                // re-add base64 padding if missing
                encoded = PadBase64String(encoded);
                // decode the base64 string
                var bytes = Convert.FromBase64String(encoded);
                var idStr = Encoding.UTF8.GetString(bytes);
                // try to parse a ulong from the resulting string
                if (ulong.TryParse(idStr, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
                    return id;
            }
            catch (DecoderFallbackException)
            {
                // ignore exception, can be thrown by GetString
            }
            catch (FormatException)
            {
                // ignore exception, can be thrown if base64 string is invalid
            }
            catch (ArgumentException)
            {
                // ignore exception, can be thrown by BitConverter, or by PadBase64String
            }
            return null;
        }

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
            // return true if the user id could be determined
            return DecodeBase64UserId(segments[0]).HasValue;
        }

        /// <summary>
        ///     The set of all characters that are not allowed inside of a token.
        /// </summary>
        internal static char[] IllegalTokenCharacters = new char[]
        {
            ' ', '\t', '\r', '\n'
        };

        /// <summary>
        ///     Checks if the given token contains a whitespace or newline character
        ///     that would fail to log in.
        /// </summary>
        /// <param name="token"> The token to validate. </param>
        /// <returns>
        ///     True if the token contains a whitespace or newline character.
        /// </returns>
        internal static bool CheckContainsIllegalCharacters(string token)
            => token.IndexOfAny(IllegalTokenCharacters) != -1;

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
            // ensure that there are no whitespace or newline characters
            if (CheckContainsIllegalCharacters(token))
                throw new ArgumentException(message: "The token contains a whitespace or newline character. Ensure that the token has been properly trimmed.", paramName: nameof(token));

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
                        throw new ArgumentException(message: $"A Bot token must be at least {MinBotTokenLength} characters in length. " +
                            "Ensure that the Bot Token provided is not an OAuth client secret.", paramName: nameof(token));
                    // check the validity of the bot token by decoding the ulong userid from the jwt
                    if (!CheckBotTokenValidity(token))
                        throw new ArgumentException(message: "The Bot token was invalid. " +
                            "Ensure that the Bot Token provided is not an OAuth client secret.", paramName: nameof(token));
                    break;
                default:
                    // All unrecognized TokenTypes (including User tokens) are considered to be invalid.
                    throw new ArgumentException(message: "Unrecognized TokenType.", paramName: nameof(token));
            }
        }
    }
}

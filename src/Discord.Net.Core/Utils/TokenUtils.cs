using System;

namespace Discord
{
    /// <summary>
    ///     Provides a series of helper methods for handling Discord login tokens.
    /// </summary>
    public static class TokenUtils
    {
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
                    // bot tokens are assumed to be at least 59 characters in length
                    // this value was determined by referencing examples in the discord documentation, and by comparing with
                    // pre-existing tokens
                    if (token.Length < 59)
                        throw new ArgumentException(message: "A Bot token must be at least 59 characters in length.", paramName: nameof(token));
                    break;
                default:
                    // All unrecognized TokenTypes (including User tokens) are considered to be invalid.
                    throw new ArgumentException(message: "Unrecognized TokenType.", paramName: nameof(token));
            }
        }

    }
}

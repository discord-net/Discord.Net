using System;

namespace Discord
{
    /// <summary>
    ///     Represents an authentication token used to authenticate with the
    ///     Discord API.
    /// </summary>
    /// <param name="Token">The authentication token value.</param>
    /// <param name="Type">The type of the token.</param>
    public readonly record struct DiscordToken(string Value, TokenType Type)
    {
        public static implicit operator DiscordToken(string token)
        {
            TokenUtils.ValidateToken(TokenType.Bot, token);
            return new DiscordToken(token, TokenType.Bot);
        }

        public static implicit operator DiscordToken((string Token, TokenType Type) value)
        {
            TokenUtils.ValidateToken(value.Type, value.Token);
            return new DiscordToken(value.Token, value.Type);
        }
    }
}


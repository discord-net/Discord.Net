using System;

namespace Discord
{
    /// <summary> Specifies the type of token to use with the client. </summary>
    public enum TokenType
    {
        /// <summary>
        ///     An OAuth2 token type.
        /// </summary>
        Bearer,
        /// <summary>
        ///     A bot token type.
        /// </summary>
        Bot,
        /// <summary>
        ///     A webhook token type.
        /// </summary>
        Webhook
    }
}

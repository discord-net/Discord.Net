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
        Webhook,
        ///<summary>
        ///     A refresh token used to get a new access token.
        /// </summary>
        Refresh,
        ///<summary>
        ///     The code used to get a Token through Oauth2
        /// </summary>
        Code

    }
}

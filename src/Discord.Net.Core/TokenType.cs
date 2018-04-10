using System;

namespace Discord
{
    public enum TokenType
    {
        [Obsolete("User logins are deprecated and may result in a ToS strike against your account - please see https://github.com/RogueException/Discord.Net/issues/827", error: true)]
        User,
        Bearer,
        Bot,
        Webhook
    }
}

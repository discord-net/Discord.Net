using Discord.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public class DiscordSocketRestClient : DiscordRestClient
    {
        internal DiscordSocketRestClient(DiscordRestConfig config, API.DiscordRestApiClient api) : base(config, api) { }

        public new Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
            => throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");
        internal override Task LoginInternalAsync(TokenType tokenType, string token, bool validateToken)
            => throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");
        public new Task LogoutAsync()
            => throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");
        internal override Task LogoutInternalAsync()
            => throw new NotSupportedException("The Socket REST wrapper cannot be used to log in or out.");
    }
}

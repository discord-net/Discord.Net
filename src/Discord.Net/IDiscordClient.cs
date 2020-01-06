using System;
using System.Net.Http.Headers;
using Discord.Rest;
using Discord.Socket;

namespace Discord
{
    public interface IDiscordClient : IDisposable
    {
        public static IDiscordClient Create(string token, DiscordConfig? config = default)
        {
            config = config ?? new DiscordConfig();

            // todo: validate token
            var tokenHeader = AuthenticationHeaderValue.Parse(token);

            var rest = new DiscordRestApi(config, tokenHeader);
            var gateway = new DiscordGatewayApi(config, token);

            return new DiscordClient(config, rest, gateway);
        }

        DiscordRestApi Rest { get; }
        DiscordGatewayApi Gateway { get; }
    }
}

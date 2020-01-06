using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.Socket;

namespace Discord
{
    internal interface IDiscordClient
    {
        static IDiscordClient Create(DiscordConfig config)
        {
            var rest = new DiscordRestApi(config);
            var gateway = new DiscordGatewayApi(config);

            return new DiscordClient(config, rest, gateway);
        }

        DiscordRestApi Rest { get; }
        DiscordGatewayApi Gateway { get; }
    }
}

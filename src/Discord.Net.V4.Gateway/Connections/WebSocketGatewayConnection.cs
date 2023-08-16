using Discord.API.Gateway;
using System;

namespace Discord.Gateway
{
    internal sealed class WebSocketGatewayConnection : IGatewayConnection
    {
        public event Func<GatewayPayload, ValueTask> PayloadReceived
        {

        }

        public Task Connect(DiscordGatewayClient client)
        {

        }


        public Task SendPayload(GatewayPayload payload)
        {

        }
    }
}


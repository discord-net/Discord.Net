using Discord.API.Gateway;
using System;

namespace Discord.Gateway
{
    internal sealed class WebSocketGatewayConnection : IGatewayConnection
    {
        public event Func<GatewayPayload, ValueTask> PayloadReceived
        {
            add => _payloadReceived.Add(value);
            remove => _payloadReceived.Remove(value);
        }

        private readonly AsyncEvent<Func<GatewayPayload, ValueTask>> _payloadReceived;

        public WebSocketGatewayConnection()
        {
            _payloadReceived = new();
        }

        public Task ConnectAsync(DiscordGatewayClient client)
        {

        }


        public Task SendPayloadAsync(GatewayPayload payload)
        {

        }
    }
}


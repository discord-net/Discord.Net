using Discord.API.Gateway;
using System;
namespace Discord.Gateway
{
    public interface IGatewayConnection
    {
        Task ConnectAsync(DiscordGatewayClient client);

        event Func<GatewayPayload, ValueTask> PayloadReceived; 
        Task SendPayloadAsync(GatewayPayload payload);
    }
}


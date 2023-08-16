using Discord.API.Gateway;
using System;
namespace Discord.Gateway
{
    internal interface IGatewayConnection
    {
        Task Connect(DiscordGatewayClient client);

        event Func<GatewayPayload, ValueTask> PayloadReceived; 
        Task SendPayload(GatewayPayload payload);
    }
}


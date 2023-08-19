using Discord.API.Gateway;
using Discord.API.Rest;
using System;
namespace Discord.Gateway
{
    public interface IGatewayConnection
    {
        Task ConnectAsync(
            GetBotGatewayResponse botGateway,
            CancellationToken token = default);

        Task DisconnectAsync(CancellationToken token = default);

        ValueTask SendPayloadAsync(in GatewayPayload payload, CancellationToken token = default);
        ValueTask<GatewayPayload> ReadPayloadAsync(CancellationToken token = default);
    }
}


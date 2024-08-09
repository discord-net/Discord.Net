using Discord.Models;

namespace Discord.Gateway.Processors;

public interface IDispatchProcessor<in TPayload> where TPayload : class, IGatewayPayloadData
{
    static abstract string DispatchEventType { get; }
    static abstract ValueTask ProcessAsync(DiscordGatewayClient client, TPayload payload, CancellationToken token = default);
}

using Discord.Models;

namespace Discord.Gateway.Events.Processors;

public interface IDispatchProcessor<in TPayload> where TPayload : class, IGatewayPayloadData
{
    string DispatchName { get; }
    ValueTask ProcessAsync(TPayload payload, CancellationToken cancellationToken = default);
}

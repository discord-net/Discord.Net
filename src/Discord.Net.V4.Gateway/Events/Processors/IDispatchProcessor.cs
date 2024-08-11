using Discord.Models;

namespace Discord.Gateway.Processors;

public interface IDispatchProcessor<in TPayload> : IDispatchProcessor
    where TPayload : class, IGatewayPayloadData
{
    ValueTask ProcessAsync(TPayload payload, CancellationToken token = default);

    Type IDispatchProcessor.PayloadType => typeof(TPayload);
    ValueTask IDispatchProcessor.ProcessAsync(IGatewayPayloadData payload, CancellationToken token)
        => payload is TPayload ourPayload ? ProcessAsync(ourPayload, token) : ValueTask.CompletedTask;
}

public interface IDispatchProcessor
{
    string DispatchEventType { get; }
    Type PayloadType { get; }
    ValueTask ProcessAsync(IGatewayPayloadData payload, CancellationToken token = default);
}

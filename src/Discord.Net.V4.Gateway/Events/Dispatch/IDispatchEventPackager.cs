using Discord.Models;

namespace Discord.Gateway;

public interface IDispatchEventPackager<TPackage, in TPayload>
    where TPayload : IGatewayPayloadData
{
    ValueTask<TPackage?> PackageAsync(TPayload? payload, CancellationToken token = default);
}

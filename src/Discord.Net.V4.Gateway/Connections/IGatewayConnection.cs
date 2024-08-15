namespace Discord.Gateway;

public interface IGatewayConnection : IAsyncDisposable
{
    Task ConnectAsync(
        Uri url,
        CancellationToken token = default);

    Task DisconnectAsync(CancellationToken token = default);

    ValueTask SendAsync(Stream stream, TransportFormat format, CancellationToken token = default);
    ValueTask<GatewayReadResult> ReadAsync(Stream stream, CancellationToken token = default);
}

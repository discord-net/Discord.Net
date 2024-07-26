namespace Discord.Gateway;

public interface IGatewayConnection : IAsyncDisposable
{
    Task ConnectAsync(
        Uri url,
        CancellationToken token = default);

    Task DisconnectAsync(CancellationToken token = default);

    ValueTask SendAsync(Stream stream, CancellationToken token = default);
    ValueTask<Stream> ReadAsync(CancellationToken token = default);
}

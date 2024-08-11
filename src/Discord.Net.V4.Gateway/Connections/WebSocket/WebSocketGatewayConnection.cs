using System.IO.Compression;
using System.Net.WebSockets;

namespace Discord.Gateway;

[method: TypeFactory]
internal sealed partial class WebSocketGatewayConnection(
    DiscordGatewayClient client,
    DiscordGatewayConfig config
) : IGatewayConnection
{
    public const int FRAME_SIZE = 2048;

    private readonly DiscordGatewayConfig _config = config;
    private readonly DiscordGatewayClient _client = client;

    private readonly ClientWebSocket _socket = new();

    private readonly RentedArray<byte> _readBuffer = config.BufferPool.RentHandle(FRAME_SIZE);
    private readonly RentedArray<byte> _writeBuffer = config.BufferPool.RentHandle(FRAME_SIZE);

    private readonly SemaphoreSlim _readSemaphore = new(1, 1);
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    public async Task ConnectAsync(Uri uri, CancellationToken token = default)
    {
        if (_socket.State is not (WebSocketState.Open or WebSocketState.Connecting))
            await _socket.ConnectAsync(uri, token);
    }

    public async Task DisconnectAsync(CancellationToken token = default)
    {
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
    }


    public async ValueTask SendAsync(Stream stream, CancellationToken token = default)
    {
        await _writeSemaphore.WaitAsync(token);

        try
        {
            int chunkSize;

            await using var compression = new GZipStream(stream, CompressionMode.Compress, false);

            while ((chunkSize = await compression.ReadAsync(_writeBuffer.Array, token)) > 0)
            {
                await _socket.SendAsync(
                    _writeBuffer.Array[..chunkSize],
                    WebSocketMessageType.Binary,
                    chunkSize != _writeBuffer.Array.Length,
                    token
                );
            }

            if (chunkSize == 0)
                await _socket.SendAsync(ArraySegment<byte>.Empty, WebSocketMessageType.Binary, true, token);
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    public async ValueTask<GatewayCloseStatus?> ReadAsync(Stream stream, CancellationToken token = default)
    {
        await _readSemaphore.WaitAsync(token);

        try
        {
            var readComplete = false;

            while (!readComplete)
            {
                var result = await _socket.ReceiveAsync(_readBuffer.Array, token);

                readComplete = result.EndOfMessage;

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary:
                        await stream.WriteAsync(_readBuffer.Array.AsMemory()[..result.Count], token);
                        break;
                    case WebSocketMessageType.Close:
                        var closeCode = result.CloseStatus.HasValue
                            ? (GatewayCloseCode)(ushort)result.CloseStatus.Value
                            : GatewayCloseCode.Unspecified;
                        return new GatewayCloseStatus(closeCode, result.CloseStatusDescription);
                    case WebSocketMessageType.Text:
                        throw new InvalidDataException("Expected binary only message");
                }
            }

            return null;
        }
        finally
        {
            _readSemaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(_socket);
        await CastAndDispose(_readBuffer);
        await CastAndDispose(_writeBuffer);
        await CastAndDispose(_readSemaphore);
        await CastAndDispose(_writeSemaphore);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}

using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway;

[method: TypeFactory]
internal sealed partial class WebSocketGatewayConnection(
    DiscordGatewayClient client,
    DiscordGatewayConfig config
) : IGatewayConnection
{
    public const int READ_FRAME_SIZE = 81920; // based off of Stream.GetCopyBufferSize 
    public const int WRITE_FRAME_SIZE = 4096; // smaller since normally, the client doesn't send large packets

    private readonly ClientWebSocket _socket = new();

    private readonly ILogger<WebSocketGatewayConnection> _logger =
        client.LoggerFactory.CreateLogger<WebSocketGatewayConnection>();

    private readonly RentedArray<byte> _readBuffer = config.BufferPool.RentHandle(READ_FRAME_SIZE);
    private readonly RentedArray<byte> _writeBuffer = config.BufferPool.RentHandle(WRITE_FRAME_SIZE);

    private readonly SemaphoreSlim _readSemaphore = new(1, 1);
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    public async Task ConnectAsync(Uri uri, CancellationToken token = default)
    {
        _logger.LogDebug("Received connection request to '{Uri}'", uri.ToString());

        if (_socket.State is not (WebSocketState.Open or WebSocketState.Connecting))
        {
            _logger.LogDebug("Opening the socket connection...");
            await _socket.ConnectAsync(uri, token);
        }
    }

    public async Task DisconnectAsync(CancellationToken token = default)
    {
        _logger.LogDebug("Closing the socket connection...");
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
    }


    public async ValueTask SendAsync(Stream stream, TransportFormat format, CancellationToken token = default)
    {
        _logger.LogDebug("Received send request, entering write semaphore");

        await _writeSemaphore.WaitAsync(token);

        try
        {
            int chunkSize;
            var didWrite = false;

            while ((chunkSize = await stream.ReadAsync(_writeBuffer.Array, token)) > 0)
            {
                _logger.LogDebug("Writing {Count} bytes as {Format}...", chunkSize, format);

                var isFinal = chunkSize != _writeBuffer.Array.Length;
                
                await _socket.SendAsync(
                    _writeBuffer.Array[..chunkSize],
                    (WebSocketMessageType)format,
                    chunkSize != _writeBuffer.Array.Length,
                    token
                );

                didWrite = true;

                if (isFinal) break;
            }

            if (chunkSize == 0 && didWrite)
            {
                _logger.LogDebug("Sending empty 'end of message' frame");
                await _socket.SendAsync(ArraySegment<byte>.Empty, (WebSocketMessageType)format, true, token);
            }
        }
        finally
        {
            _writeSemaphore.Release();
            _logger.LogDebug("Released the write semaphore");
        }
    }

    public async ValueTask<GatewayReadResult> ReadAsync(Stream stream, CancellationToken token = default)
    {
        _logger.LogDebug("Received read request, entering read semaphore");

        await _readSemaphore.WaitAsync(token);

        try
        {
            var readComplete = false;

            var messageType = WebSocketMessageType.Close;

            while (!readComplete)
            {
                _logger.LogDebug("Waiting for websocket message...");
                var result = await _socket.ReceiveAsync(_readBuffer.Array, token);

                readComplete = result.EndOfMessage;

                _logger.LogDebug(
                    "Received {Type}, {Count} bytes, is final?: {IsFinal}",
                    result.MessageType,
                    result.Count,
                    result.EndOfMessage
                );

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary or WebSocketMessageType.Text:
                        await stream.WriteAsync(_readBuffer.Array.AsMemory()[..result.Count], token);
                        messageType = result.MessageType;
                        break;
                    case WebSocketMessageType.Close:
                        var closeCode = result.CloseStatus.HasValue
                            ? (GatewayCloseCode) (ushort) result.CloseStatus.Value
                            : GatewayCloseCode.Unspecified;
                        return new GatewayReadResult(
                            TransportFormat.Binary,
                            closeCode,
                            result.CloseStatusDescription
                        );
                }
            }

            return new((TransportFormat)messageType);
        }
        finally
        {
            _readSemaphore.Release();
            _logger.LogDebug("Released the read semaphore");
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
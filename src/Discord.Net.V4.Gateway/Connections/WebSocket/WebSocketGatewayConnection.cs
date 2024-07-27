using Discord.API.Gateway;
using System;
using System.Buffers;
using System.Drawing;
using System.IO.Compression;
using System.Net.WebSockets;

namespace Discord.Gateway;

[method: TypeFactory]
internal sealed partial class WebSocketGatewayConnection(DiscordGatewayClient client, DiscordGatewayConfig config)
    : IGatewayConnection
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

    public async ValueTask<Stream> ReadAsync(CancellationToken token = default)
    {
        await _readSemaphore.WaitAsync(token);

        try
        {
            var memoryStream = new MemoryStream(FRAME_SIZE);
            var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress, false);
            var readComplete = false;

            while (!readComplete)
            {
                var result = await _socket.ReceiveAsync(_readBuffer.Array, token);

                readComplete = result.EndOfMessage;

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary:
                        await memoryStream.WriteAsync(_readBuffer.Array[..result.Count], token);
                        break;
                    case WebSocketMessageType.Close:
                        // TODO
                        break;
                    case WebSocketMessageType.Text:
                        throw new InvalidDataException("Expected binary only message");
                }
            }

            return gzipStream;
        }
        finally
        {
            _readSemaphore.Release();
        }


        // var buffers = new List<FrameStream.BufferSource>();
        //
        // var activeBuffer = _config.BufferPool.RentHandle(FRAME_SIZE);
        //
        // var fullMessage = false;
        // var size = 0;
        // while (!fullMessage)
        // {
        //     var result = await _socket.ReceiveAsync(activeBuffer.Array, token);
        //
        //     switch (result.MessageType)
        //     {
        //         case WebSocketMessageType.Binary:
        //             buffers.Add(new FrameStream.BufferSource(activeBuffer, result.Count));
        //
        //             if (!result.EndOfMessage)
        //                 activeBuffer = _config.BufferPool.RentHandle(FRAME_SIZE);
        //             break;
        //         case WebSocketMessageType.Close:
        //             // TODO
        //             break;
        //         case WebSocketMessageType.Text:
        //             throw new InvalidDataException("Expected binary only message");
        //     }
        //
        //     fullMessage = result.EndOfMessage;
        //     size += result.Count;
        // }
        //
        // return new GZipStream(new FrameStream(buffers, size), CompressionMode.Decompress, false);
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

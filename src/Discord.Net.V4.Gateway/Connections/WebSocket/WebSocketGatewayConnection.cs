using Discord.API.Gateway;
using Discord.API.Rest;
using System;
using System.Buffers;
using System.Drawing;
using System.IO.Compression;
using System.Net.WebSockets;

namespace Discord.Gateway
{
    internal sealed class WebSocketGatewayConnection : IGatewayConnection
    {
        public const int FRAME_SIZE = 2048;

        private readonly DiscordGatewayConfig _config;
        private readonly DiscordGatewayClient _client;
        private readonly ClientWebSocket _socket;

        public WebSocketGatewayConnection(DiscordGatewayClient client, DiscordGatewayConfig config)
        {
            _client = client;
            _config = config;
            _socket = new ClientWebSocket();
        }

        public async Task ConnectAsync(Uri uri, CancellationToken token = default(CancellationToken))
        {
            if(!(_socket.State is WebSocketState.Open or WebSocketState.Connecting))
                await _socket.ConnectAsync(new Uri(uri + $"?v={_config.GatewayVersion}&encoding=etf&compress=zlib-stream"), token);
        }

        public async Task DisconnectAsync(CancellationToken token = default(CancellationToken))
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
        }

        public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default(CancellationToken))
            => _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, token);

        public async ValueTask<Stream> ReadAsync(CancellationToken token = default(CancellationToken))
        {
            var buffers = new List<FrameStream.BufferSource>();

            var activeBuffer = _config.BufferPool.RentHandle(FRAME_SIZE);

            var fullMessage = false;
            var size = 0;
            while(!fullMessage)
            {
                var result = await _socket.ReceiveAsync(activeBuffer.Array, token);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary:
                        buffers.Add(new FrameStream.BufferSource(activeBuffer, result.Count));

                        if(!result.EndOfMessage)
                            activeBuffer = _config.BufferPool.RentHandle(FRAME_SIZE);
                        break;
                    case WebSocketMessageType.Close:
                        // TODO
                        break;
                    case WebSocketMessageType.Text:
                        throw new InvalidDataException("Expected binary only message");
                }

                fullMessage = result.EndOfMessage;
                size += result.Count;
            }

            return new GZipStream(new FrameStream(buffers, size), CompressionMode.Decompress, false);
        }

        public static GatewayConnectionFactory Factory
            => (client, config) => new WebSocketGatewayConnection(client, config);
    }
}


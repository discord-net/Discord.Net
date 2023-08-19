using Discord.API.Gateway;
using Discord.API.Rest;
using System;
using System.Buffers;
using System.Drawing;
using System.Net.WebSockets;

namespace Discord.Gateway
{
    internal sealed class WebSocketGatewayConnection : IGatewayConnection
    {
        public const int FRAME_SIZE = 2048;

        private readonly DiscordGatewayConfig _config;
        private readonly DiscordGatewayClient _client;
        private readonly ClientWebSocket _socket;
        private readonly MemoryPool<byte> _memoryPool;

        public WebSocketGatewayConnection(DiscordGatewayClient client, DiscordGatewayConfig config)
        {
            _memoryPool = MemoryPool<byte>.Shared;
            _client = client;
            _config = config;
            _socket = new ClientWebSocket();
        }

        public async Task ConnectAsync(GetBotGatewayResponse botGateway, CancellationToken token = default(CancellationToken))
        {
            if(!(_socket.State is WebSocketState.Open or WebSocketState.Connecting))
                await _socket.ConnectAsync(new Uri(botGateway.Url + $"?v={_config.GatewayVersion}&encoding=etf"), token);
        }

        public async Task DisconnectAsync(CancellationToken token = default(CancellationToken))
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
        }

        public ValueTask SendPayloadAsync(in GatewayPayload payload, CancellationToken token = default(CancellationToken))
        {
            var buffer = ETFPack.Encode(in payload);

            return _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, token);
        }

        public async ValueTask<GatewayPayload> ReadPayloadAsync(CancellationToken token = default(CancellationToken))
        {
            var buffers = new List<FrameSource.BufferSource>();

            var activeBuffer = _memoryPool.Rent(FRAME_SIZE);

            var fullMessage = false;
            int size = 0;
            while(!fullMessage)
            {
                var result = await _socket.ReceiveAsync(activeBuffer.Memory, token);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary:
                        buffers.Add(new FrameSource.BufferSource(activeBuffer, result.Count));
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

            return DecodeBinaryMessage(buffers, size);
        }

        private GatewayPayload DecodeBinaryMessage(List<FrameSource.BufferSource> buffers, int size)
        {
            using var frameSource = new FrameSource(buffers, size);

        }


        public static GatewayConnectionFactory Factory
            => (client, config) => new WebSocketGatewayConnection(client, config);
    }
}


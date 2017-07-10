using Discord.API;
using Discord.API.Gateway;
using Discord.Logging;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClient = System.Net.WebSockets.WebSocket;

namespace Discord.Relay
{
    public class RelayConnection
    {
        private readonly RelayServer _server;
        private readonly WebSocketClient _socket;
        private readonly CancellationTokenSource _cancelToken;
        private readonly byte[] _inBuffer, _outBuffer;
        private readonly Logger _logger;

        internal RelayConnection(RelayServer server, WebSocketClient socket, int id)
        {
            _server = server;
            _socket = socket;
            _cancelToken = new CancellationTokenSource();
            _inBuffer = new byte[4000];
            _outBuffer = new byte[4000];
            _logger = server.LogManager.CreateLogger($"Client #{id}");
        }

        internal async Task RunAsync()
        {
            await _logger.InfoAsync($"Connected");
            var token = _cancelToken.Token;
            try
            {
                var segment = new ArraySegment<byte>(_inBuffer);

                //Send HELLO
                await SendAsync(GatewayOpCode.Hello, new HelloEvent { HeartbeatInterval = 15000 }).ConfigureAwait(false);

                while (_socket.State == WebSocketState.Open)
                {
                    var result = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);
                    if (result.MessageType == WebSocketMessageType.Close)
                        await _logger.WarningAsync($"Received Close {result.CloseStatus} ({result.CloseStatusDescription ?? "No Reason"})").ConfigureAwait(false);
                    else
                        await _logger.InfoAsync($"Received {result.Count} bytes");
                }
            }
            catch (OperationCanceledException) 
            { 
                try { await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false); }
                catch { }
            }
            catch (Exception ex) 
            { 
                try { await _socket.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None).ConfigureAwait(false); }
                catch { }
            }
            finally
            {
                await _logger.InfoAsync($"Disconnected");
            }
        }

        internal void Stop()
        {
            _cancelToken.Cancel();
        }

        private async Task SendAsync(GatewayOpCode opCode, object payload)
        {
            var frame = new SocketFrame { Operation = (int)opCode, Payload = payload };
            var bytes = _server.Serialize(frame, _outBuffer);
            var segment = new ArraySegment<byte>(_outBuffer, 0, bytes);
            await _socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

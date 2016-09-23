using Discord.API.Rpc;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Rpc
{
    public partial class DiscordRpcClient : DiscordRestClient
    {
        private readonly ILogger _rpcLogger;
        private readonly JsonSerializer _serializer;

        private TaskCompletionSource<bool> _connectTask;
        private CancellationTokenSource _cancelToken, _reconnectCancelToken;
        private Task _reconnectTask;
        private bool _canReconnect;

        public ConnectionState ConnectionState { get; private set; }

        //From DiscordRpcConfig
        internal int ConnectionTimeout { get; private set; }

        public new API.DiscordRpcApiClient ApiClient => base.ApiClient as API.DiscordRpcApiClient;

        /// <summary> Creates a new RPC discord client. </summary>
        public DiscordRpcClient(string clientId, string origin) : this(new DiscordRpcConfig(clientId, origin)) { }
        /// <summary> Creates a new RPC discord client. </summary>
        public DiscordRpcClient(DiscordRpcConfig config)
            : base(config, CreateApiClient(config))
        {
            ConnectionTimeout = config.ConnectionTimeout;
            _rpcLogger = LogManager.CreateLogger("RPC");

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _rpcLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };
            
            ApiClient.SentRpcMessage += async opCode => await _rpcLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
            ApiClient.ReceivedRpcEvent += ProcessMessageAsync;
            ApiClient.Disconnected += async ex =>
            {
                if (ex != null)
                {
                    await _rpcLogger.WarningAsync($"Connection Closed", ex).ConfigureAwait(false);
                    await StartReconnectAsync(ex).ConfigureAwait(false);
                }
                else
                    await _rpcLogger.WarningAsync($"Connection Closed").ConfigureAwait(false);
            };
        }
        private static API.DiscordRpcApiClient CreateApiClient(DiscordRpcConfig config)
            => new API.DiscordRpcApiClient(config.ClientId, config.Origin, config.RestClientProvider, config.WebSocketProvider, requestQueue: new RequestQueue());

        internal override void Dispose(bool disposing)
        {
            if (!_isDisposed)
                ApiClient.Dispose();
        }

        protected override Task ValidateTokenAsync(TokenType tokenType, string token)
        {
            return Task.CompletedTask; //Validation is done in DiscordRpcAPIClient
        }

        /// <inheritdoc />
        public Task ConnectAsync() => ConnectAsync(false);
        internal async Task ConnectAsync(bool ignoreLoginCheck)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(ignoreLoginCheck, false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync(bool ignoreLoginCheck, bool isReconnecting)
        {
            if (!ignoreLoginCheck && LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");
            
            if (!isReconnecting && _reconnectCancelToken != null && !_reconnectCancelToken.IsCancellationRequested)
                _reconnectCancelToken.Cancel();

            var state = ConnectionState;
            if (state == ConnectionState.Connecting || state == ConnectionState.Connected)
                await DisconnectInternalAsync(null, isReconnecting).ConfigureAwait(false);

            ConnectionState = ConnectionState.Connecting;
            await _rpcLogger.InfoAsync("Connecting").ConfigureAwait(false);
            try
            {
                var connectTask = new TaskCompletionSource<bool>();
                _connectTask = connectTask;
                _cancelToken = new CancellationTokenSource();

                //Abort connection on timeout
                var _ = Task.Run(async () =>
                {
                    await Task.Delay(ConnectionTimeout);
                    connectTask.TrySetException(new TimeoutException());
                });

                await ApiClient.ConnectAsync().ConfigureAwait(false);
                await _connectedEvent.InvokeAsync().ConfigureAwait(false);

                await _connectTask.Task.ConfigureAwait(false);
                if (!isReconnecting)
                    _canReconnect = true;
                ConnectionState = ConnectionState.Connected;
                await _rpcLogger.InfoAsync("Connected").ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectInternalAsync(null, isReconnecting).ConfigureAwait(false);
                throw;
            }
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (_connectTask?.TrySetCanceled() ?? false) return;
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(null, false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync(Exception ex, bool isReconnecting)
        {
            if (!isReconnecting)
            {
                _canReconnect = false;

                if (_reconnectCancelToken != null && !_reconnectCancelToken.IsCancellationRequested)
                    _reconnectCancelToken.Cancel();
            }

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            await _rpcLogger.InfoAsync("Disconnecting").ConfigureAwait(false);

            await _rpcLogger.DebugAsync("Disconnecting - CancelToken").ConfigureAwait(false);
            //Signal tasks to complete
            try { _cancelToken.Cancel(); } catch { }

            await _rpcLogger.DebugAsync("Disconnecting - ApiClient").ConfigureAwait(false);
            //Disconnect from server
            await ApiClient.DisconnectAsync().ConfigureAwait(false);
            
            ConnectionState = ConnectionState.Disconnected;
            await _rpcLogger.InfoAsync("Disconnected").ConfigureAwait(false);

            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        }

        private async Task StartReconnectAsync(Exception ex)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_canReconnect || _reconnectTask != null) return;
                _reconnectCancelToken = new CancellationTokenSource();
                _reconnectTask = ReconnectInternalAsync(ex, _reconnectCancelToken.Token);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ReconnectInternalAsync(Exception ex, CancellationToken cancelToken)
        {
            if (ex == null)
            {
                if (_connectTask?.TrySetCanceled() ?? false) return;
            }
            else
            {
                if (_connectTask?.TrySetException(ex) ?? false) return;
            }

            try
            {
                Random jitter = new Random();
                int nextReconnectDelay = 1000;
                while (true)
                {
                    await Task.Delay(nextReconnectDelay, cancelToken).ConfigureAwait(false);
                    nextReconnectDelay = nextReconnectDelay * 2 + jitter.Next(-250, 250);
                    if (nextReconnectDelay > 60000)
                        nextReconnectDelay = 60000;

                    await _connectionLock.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        if (cancelToken.IsCancellationRequested) return;
                        await ConnectInternalAsync(false, true).ConfigureAwait(false);
                        _reconnectTask = null;
                        return;
                    }
                    catch (Exception ex2)
                    {
                        await _rpcLogger.WarningAsync("Reconnect failed", ex2).ConfigureAwait(false);
                    }
                    finally { _connectionLock.Release(); }
                }
            }
            catch (OperationCanceledException)
            {
                await _connectionLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    await _rpcLogger.DebugAsync("Reconnect cancelled").ConfigureAwait(false);
                    _reconnectTask = null;
                }
                finally { _connectionLock.Release(); }
            }
        }

        public async Task<string> AuthorizeAsync(string[] scopes, string rpcToken = null)
        {
            await ConnectAsync(true).ConfigureAwait(false);
            var result = await ApiClient.SendAuthorizeAsync(scopes, rpcToken).ConfigureAwait(false);
            await DisconnectAsync().ConfigureAwait(false);
            return result.Code;
        }

        public async Task SubscribeGuild(ulong guildId, params RpcChannelEvent[] events)
        {
            Preconditions.AtLeast(events?.Length ?? 0, 1, nameof(events));
            for (int i = 0; i < events.Length; i++)
                await ApiClient.SendGuildSubscribeAsync(GetEventName(events[i]), guildId);
        }
        public async Task UnsubscribeGuild(ulong guildId, params RpcChannelEvent[] events)
        {
            Preconditions.AtLeast(events?.Length ?? 0, 1, nameof(events));
            for (int i = 0; i < events.Length; i++)
                await ApiClient.SendGuildUnsubscribeAsync(GetEventName(events[i]), guildId);
        }
        public async Task SubscribeChannel(ulong channelId, params RpcChannelEvent[] events)
        {
            Preconditions.AtLeast(events?.Length ?? 0, 1, nameof(events));
            for (int i = 0; i < events.Length; i++)
                await ApiClient.SendChannelSubscribeAsync(GetEventName(events[i]), channelId);
        }
        public async Task UnsubscribeChannel(ulong channelId, params RpcChannelEvent[] events)
        {
            Preconditions.AtLeast(events?.Length ?? 0, 1, nameof(events));
            for (int i = 0; i < events.Length; i++)
                await ApiClient.SendChannelUnsubscribeAsync(GetEventName(events[i]), channelId);
        }

        private static string GetEventName(RpcGuildEvent rpcEvent)
        {
            switch (rpcEvent)
            {
                case RpcGuildEvent.GuildStatus: return "GUILD_STATUS";
                default:
                    throw new InvalidOperationException($"Unknown RPC Guild Event: {rpcEvent}");
            }
        }
        private static string GetEventName(RpcChannelEvent rpcEvent)
        {
            switch (rpcEvent)
            {
                case RpcChannelEvent.VoiceStateCreate: return "VOICE_STATE_CREATE";
                case RpcChannelEvent.VoiceStateUpdate: return "VOICE_STATE_UPDATE";
                case RpcChannelEvent.VoiceStateDelete: return "VOICE_STATE_DELETE";
                case RpcChannelEvent.SpeakingStart: return "SPEAKING_START";
                case RpcChannelEvent.SpeakingStop: return "SPEAKING_STOP";
                case RpcChannelEvent.MessageCreate: return "MESSAGE_CREATE";
                case RpcChannelEvent.MessageUpdate: return "MESSAGE_UPDATE";
                case RpcChannelEvent.MessageDelete: return "MESSAGE_DELETE";
                default:
                    throw new InvalidOperationException($"Unknown RPC Channel Event: {rpcEvent}");
            }
        }

        private async Task ProcessMessageAsync(string cmd, Optional<string> evnt, Optional<object> payload)
        {
            try
            {
                switch (cmd)
                {
                    case "DISPATCH":
                        switch (evnt.Value)
                        {
                            //Connection
                            case "READY":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (READY)").ConfigureAwait(false);
                                    var data = (payload.Value as JToken).ToObject<ReadyEvent>(_serializer);
                                    var cancelToken = _cancelToken;

                                    var _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            RequestOptions options = new RequestOptions
                                            {
                                                //CancellationToken = cancelToken //TODO: Implement
                                            };

                                            if (LoginState != LoginState.LoggedOut)
                                                await ApiClient.SendAuthenticateAsync(options).ConfigureAwait(false); //Has bearer

                                            var __ = _connectTask.TrySetResultAsync(true); //Signal the .Connect() call to complete
                                            await _rpcLogger.InfoAsync("Ready").ConfigureAwait(false);
                                        }
                                        catch (Exception ex)
                                        {
                                            await _rpcLogger.ErrorAsync($"Error handling {cmd}{(evnt.IsSpecified ? $" ({evnt})" : "")}", ex).ConfigureAwait(false);
                                            return;
                                        }
                                    });
                                }
                                break;

                            //Guilds
                            case "GUILD_STATUS":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (GUILD_STATUS)").ConfigureAwait(false);

                                    await _guildUpdatedEvent.InvokeAsync().ConfigureAwait(false);
                                }
                                break;

                            //Voice
                            case "VOICE_STATE_CREATE":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (VOICE_STATE_CREATE)").ConfigureAwait(false);

                                    await _voiceStateUpdatedEvent.InvokeAsync().ConfigureAwait(false);
                                }
                                break;
                            case "VOICE_STATE_UPDATE":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (VOICE_STATE_UPDATE)").ConfigureAwait(false);

                                    await _voiceStateUpdatedEvent.InvokeAsync().ConfigureAwait(false);
                                }
                                break;
                            case "VOICE_STATE_DELETE":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (VOICE_STATE_DELETE)").ConfigureAwait(false);

                                    await _voiceStateUpdatedEvent.InvokeAsync().ConfigureAwait(false);
                                }
                                break;

                            case "SPEAKING_START":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (SPEAKING_START)").ConfigureAwait(false);
                                    await _voiceStateUpdatedEvent.InvokeAsync().ConfigureAwait(false);
                                }
                                break;
                            case "SPEAKING_STOP":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (SPEAKING_STOP)").ConfigureAwait(false);

                                    await _voiceStateUpdatedEvent.InvokeAsync().ConfigureAwait(false);
                                }
                                break;

                            //Messages
                            case "MESSAGE_CREATE":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (MESSAGE_CREATE)").ConfigureAwait(false);
                                    var data = (payload.Value as JToken).ToObject<MessageEvent>(_serializer);
                                    var msg = new RpcMessage(this, data.Message);

                                    await _messageReceivedEvent.InvokeAsync(data.ChannelId, msg).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_UPDATE":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (MESSAGE_UPDATE)").ConfigureAwait(false);
                                    var data = (payload.Value as JToken).ToObject<MessageEvent>(_serializer);
                                    var msg = new RpcMessage(this, data.Message);

                                    await _messageUpdatedEvent.InvokeAsync(data.ChannelId, msg).ConfigureAwait(false);
                                }
                                break;
                            case "MESSAGE_DELETE":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE)").ConfigureAwait(false);
                                    var data = (payload.Value as JToken).ToObject<MessageEvent>(_serializer);

                                    await _messageDeletedEvent.InvokeAsync(data.ChannelId, data.Message.Id).ConfigureAwait(false);
                                }
                                break;

                            //Others
                            default:
                                await _rpcLogger.WarningAsync($"Unknown Dispatch ({evnt})").ConfigureAwait(false);
                                return;
                        }
                        break;

                    /*default: //Other opcodes are used for command responses
                        await _rpcLogger.WarningAsync($"Unknown OpCode ({cmd})").ConfigureAwait(false);
                        return;*/
                }
            }
            catch (Exception ex)
            {
                await _rpcLogger.ErrorAsync($"Error handling {cmd}{(evnt.IsSpecified ? $" ({evnt})" : "")}", ex).ConfigureAwait(false);
                return;
            }
        }
    }
}

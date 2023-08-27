using Discord.API;
using Discord.API.Gateway;
using Discord.Gateway.Cache;
using Discord.Gateway.EventProcessors;
using Discord.Gateway.State;
using Discord.Logging;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public class DiscordGatewayClient : IGatewayClient
    {
        public bool IsConnected { get; private set; }

        public TimeSpan Ping { get; private set; }

        internal StateController State { get; }

        internal DiscordGatewayConfig Config { get; }

        internal DiscordRestClient Rest { get; }

        internal ILogger<DiscordGatewayClient> Logger { get; }

        internal readonly ICacheProvider CacheProvider;
        internal readonly IGatewayConnection Connection;
        internal readonly IGatewayEncoding Encoding;

        private readonly ConcurrentQueue<DateTimeOffset> _heartbeats;

        private readonly DiscordToken _token;

        private readonly int _maxClientMessageTimeout;

        private readonly SemaphoreSlim _sendGatewayLock;
        private readonly SemaphoreSlim _readGatewayLock;

        private readonly TaskCompletionSource _connectionTaskSource;
        private readonly CancellationTokenSource _connectionLifecycleTokenSource;

        private readonly int? _sequence;
        private readonly Task _lifecycleTask;

        private string? _sessionId;
        private string? _resumeUrl;

        private string? _gatewayUrl;

        private int _heartbeatInterval = -1;

        public DiscordGatewayClient(
            ILogger<DiscordGatewayClient> logger,
            DiscordGatewayConfig config)
        {
            Logger = logger;
            _heartbeats = new();
            _gatewayUrl = config.CustomGatewayUrl;
            _token = config.Token;
            _maxClientMessageTimeout = config.MaxClientMessageTimeout;
            _connectionTaskSource = new();
            _connectionLifecycleTokenSource = new();
            CacheProvider = config.CacheProvider;
            Encoding = config.Encoding;
            Connection = config.GatewayConnection(this, config);
            _sendGatewayLock = new(1, 1);
            _readGatewayLock = new(1, 1);

            Config = config;
            Rest = new DiscordRestClient(config);
            State = new(this, in CacheProvider);
            _lifecycleTask = Task.Run(RunLifecycleAsync);
        }

        public DiscordGatewayClient(DiscordGatewayConfig config)
            : this(NullLogger<DiscordGatewayClient>.Instance, config)
        { }

        private async Task ProcessGatewayMessageAsync(GatewayPayload message)
        {
            switch (message.Operation)
            {
                case GatewayOpCode.Dispatch when message.Type is not null:
                    await EventProcessor.ProcessEventAsync(
                    this,
                    message.Type,
                        message.Payload
                    );
                    break;
                case GatewayOpCode.Heartbeat:
                    await SendHeartbeatAsync(_connectionLifecycleTokenSource.Token);
                    break;
                case GatewayOpCode.Reconnect:
                    // TODO
                    break;
                case GatewayOpCode.InvalidSession:
                    // TODO
                    break;
                case GatewayOpCode.Hello:
                    var helloPayload = Encoding.ToObject<HelloPayload>(message.Payload)
                        ?? throw new NullReferenceException("Got null payload on HELLO");

                    _heartbeatInterval = helloPayload.HeartbeatInterval;
                    break;
                case GatewayOpCode.HeartbeatAck:
                    var time = DateTimeOffset.UtcNow;

                    if(_heartbeats.TryDequeue(out var sentTime))
                        Ping = time - sentTime;
                    break;

                case GatewayOpCode.GuildSync:
                case GatewayOpCode.RequestGuildMembers:
                case GatewayOpCode.Resume:
                case GatewayOpCode.VoiceServerPing:
                case GatewayOpCode.VoiceStateUpdate:
                case GatewayOpCode.PresenceUpdate:
                case GatewayOpCode.Identify:
                    // TODO: log that we attempted to process outbound message
                    break;
            }
        }

        private async Task RunLifecycleAsync()
        {
            await _connectionTaskSource.Task;

            Task heartbeatTask = Task.CompletedTask;

            while (!_connectionLifecycleTokenSource.IsCancellationRequested)
            {
                if(_heartbeatInterval is -1)
                {
                    await ProcessGatewayMessageAsync(
                        await GetNextGatewayMessageAsync()
                    );

                    continue;
                }

                using var nextMessageCancellationTokenSource = new CancellationTokenSource();

                heartbeatTask = heartbeatTask.IsCompleted
                    ? Task.Delay((int)(_heartbeatInterval * Random.Shared.NextDouble()))
                    : heartbeatTask;

                using var nextMessageTask = GetNextGatewayMessageAsync(nextMessageCancellationTokenSource.Token);

                await Task.WhenAny(
                    heartbeatTask,
                    nextMessageTask
                );

                if(heartbeatTask.IsCompletedSuccessfully)
                {
                    await SendHeartbeatAsync(_connectionLifecycleTokenSource.Token);
                }

                if (nextMessageTask.IsCompletedSuccessfully)
                {
                    await ProcessGatewayMessageAsync(nextMessageTask.Result);
                }
                else if (!nextMessageTask.IsCanceled && !nextMessageTask.IsFaulted)
                    nextMessageCancellationTokenSource.Cancel();
            }
        }

        private async Task<GatewayPayload> GetNextGatewayMessageAsync(CancellationToken token = default)
        {
            using var timeoutToken = new CancellationTokenSource(_maxClientMessageTimeout);
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _connectionLifecycleTokenSource.Token,
                timeoutToken.Token,
                token
            );

            try
            {
                return await ReadGatewayMessageAsync(tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (timeoutToken.IsCancellationRequested)
                {
                    // TODO: LOG and disconnect
                }

                throw;
            }
        }


        #region Send/Recieve
        private async Task SendHeartbeatAsync(CancellationToken token)
        {
            await SendGatewayMessageAsync(GatewayOpCode.Heartbeat, _sequence, token);

            _heartbeats.Enqueue(DateTimeOffset.UtcNow);

            if(_heartbeats.Count > Config.MaxUnacknowledgedHeartbeats)
            {
                // zombified connection
            }
        }

        private Task SendIdentityAsync(IdentityPayload payload, CancellationToken token)
            => SendGatewayMessageAsync(GatewayOpCode.Identify, payload, token);

        private Task SendResumeAsync(ResumePayload payload, CancellationToken token)
            => SendGatewayMessageAsync(GatewayOpCode.Resume, payload, token);

        internal async Task SendGatewayMessageAsync(GatewayOpCode code, object? payload, CancellationToken token)
        {
            await _sendGatewayLock.WaitAsync(token);

            try
            {
                var encoded = Encoding.Encode(new GatewayPayload
                {
                    Operation = code,
                    Payload = payload
                });

                await Connection.SendAsync(encoded, token);
            }
            finally
            {
                _sendGatewayLock.Release();
            }
        }

        internal async Task<GatewayPayload> ReadGatewayMessageAsync(CancellationToken token)
        {
            await _readGatewayLock.WaitAsync(token);

            try
            {
                using var dataStream = await Connection.ReadAsync(token);
                return Encoding.Decode<GatewayPayload>(dataStream);
            }
            finally
            {
                _readGatewayLock.Release();
            }
        }
        #endregion

        #region Connect/Disconnect
        public async ValueTask ConnectAsync(CancellationToken token = default)
        {
            if (IsConnected)
                return;

            await ConnectInternalAsync(new Uri(
                _resumeUrl ?? (_gatewayUrl ??= (await Rest.ApiClient.GetGatewayAsync(token)).Url)),
                token
            );
        }

        private async Task ConnectInternalAsync(Uri uri, CancellationToken token = default)
        {
            try
            {
                await Connection.ConnectAsync(uri, token);
                IsConnected = true;
                _connectionTaskSource.SetResult();

                if(_sessionId is null)
                {
                    await SendIdentityAsync(new IdentityPayload(
                        _token.Value,
                        new IdentityConnectionProperties(
                            Environment.OSVersion.Platform.ToString(),
                            "Discord.Net V4",
                            "Discord.Net V4"
                        ),
                        (int)Config.Intents
                    ), token);
                }
                else
                {
                    await SendResumeAsync(new ResumePayload(
                        _token.Value,
                        _sessionId,
                        _sequence ?? 0
                    ), token);
                }
            }
            catch (Exception x)
            {
                _connectionTaskSource.SetException(x);
            }
        }


        #endregion

    }
}

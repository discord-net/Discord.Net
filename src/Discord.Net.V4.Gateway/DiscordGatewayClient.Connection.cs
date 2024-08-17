using System.Diagnostics;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Threading.Channels;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    private enum HeartbeatSignal
    {
        Requested,
        ReceivedAck
    }

    internal static readonly RecyclableMemoryStreamManager StreamManager = new();

    public bool IsConnected { get; private set; }

    public int? ShardId { get; private set; }
    public int? TotalShards { get; private set; }

    private IGatewayConnection? _connection;
    private Task? _eventProcessorTask;
    private CancellationTokenSource _eventProcessorCancellationTokenSource = new();

    [MemberNotNullWhen(true, nameof(_sessionId), nameof(_resumeGatewayUrl), nameof(_sequence))]
    private bool CanResume
        => _sessionId is not null && _resumeGatewayUrl is not null && _sequence.HasValue;

    private string? _resumeGatewayUrl;
    private string? _sessionId;
    private int? _sequence;
    private int _heartbeatInterval;
    private readonly object _sequenceSyncRoot = new();

    private IGatewayCompression? _compression;
    
    private readonly Channel<HeartbeatSignal> _heartbeatSignal;

    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);
    
    private void StartEventProcessor()
    {
        _eventProcessorCancellationTokenSource.Cancel();

        if (_eventProcessorTask?.IsCompleted ?? false)
            _eventProcessorTask.Dispose();

        _eventProcessorCancellationTokenSource.Dispose();
        _eventProcessorCancellationTokenSource = new();

        _eventProcessorTask = Task.Run(EventProcessorLoopAsync);
    }

    private async ValueTask<Uri> GetGatewayUriAsync(
        bool shouldResume = true,
        CancellationToken token = default)
    {
        string url;

        if (shouldResume && CanResume)
        {
            url = _resumeGatewayUrl;
        }
        else
        {
            var getGatewayResponse = await Rest.RestApiClient.ExecuteAsync(
                Routes.GetGateway,
                DefaultRequestOptions,
                token
            );

            if (getGatewayResponse is null)
                throw new NullReferenceException("get gateway was null");

            url = getGatewayResponse.Url;

            _resumeGatewayUrl = null;
            _sessionId = null;
            _sequence = null;
        }

        var uriBuilder =
            new StringBuilder($"{url}?v={Config.GatewayVersion}&encoding={Encoding.Identifier}");

        if (_compression is not null)
            uriBuilder.Append($"&compress={_compression.Identifier}");

        return new Uri(uriBuilder.ToString());
    }

    public async Task ConnectAsync(CancellationToken token = default)
    {
        _logger.LogDebug("Connection requested, entering connection semaphore...");
        await _connectionSemaphore.WaitAsync(token);

        try
        {
            if (IsConnected)
            {
                // TODO: possibly throw instead of return
                _logger.LogDebug("Exiting connection request early, we're already connected");
                return;
            }

            await ConnectInternalAsync(token: token);
        }
        finally
        {
            _connectionSemaphore.Release();
            _logger.LogDebug("Released connection semaphore from connection request");
        }
    }

    public async ValueTask DisconnectAsync(CancellationToken token = default)
    {
        _logger.LogDebug("Disconnect requested, entering connection semaphore...");
        await _connectionSemaphore.WaitAsync(token);

        try
        {
            await StopGatewayConnectionAsync(true, token);
        }
        finally
        {
            _connectionSemaphore.Release();
            _logger.LogDebug("Released connection semaphore from disconnect request");
        }
    }

    public ValueTask ReconnectAsync(CancellationToken token = default)
        => ReconnectInternalAsync(token: token);

    private async ValueTask ReconnectInternalAsync(
        bool shouldResume = true,
        bool gracefulDisconnect = true,
        CancellationToken token = default)
    {
        _logger.LogDebug(
            "Starting reconnect, resuming: {ShouldResume}, graceful disconnect: {Graceful}",
            shouldResume,
            gracefulDisconnect
        );

        await _connectionSemaphore.WaitAsync(token);

        try
        {
            await StopGatewayConnectionAsync(gracefulDisconnect, token);
            await ConnectInternalAsync(shouldResume, token);
        }
        finally
        {
            _connectionSemaphore.Release();
            _logger.LogDebug("Released connection semaphore from reconnect request");
        }
    }

    private async Task ConnectInternalAsync(bool shouldResume = true, CancellationToken token = default)
    {
        _connection ??= Config.GatewayConnection.Get(this);

        _compression?.Dispose();
        _compression = TransportCompression?.Get(this, cache: false);
        
        var gatewayUri = await GetGatewayUriAsync(shouldResume, token);

        _logger.LogInformation("Connecting to {GatewayUri}...", gatewayUri);
        await _connection.ConnectAsync(gatewayUri, token);

        IsConnected = true;

        StartEventProcessor();
    }

    private async Task EventProcessorLoopAsync()
    {
        Task? heartbeatTask = null;

        try
        {
            using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            using var initialMessageCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _eventProcessorCancellationTokenSource.Token,
                timeoutTokenSource.Token
            );

            if (CanResume)
            {
                _logger.LogInformation("Attempting to resume the session");

                await SendMessageAsync(
                    CreateResumeMessage(_sessionId, _sequence.Value),
                    _eventProcessorCancellationTokenSource.Token
                );

                var dispatchQueue = new Queue<IGatewayMessage>();

                while (true)
                {
                    var message = await ReceiveGatewayMessageAsync(
                        dispatchQueue.Count == 0
                            ? initialMessageCancellationTokenSource.Token
                            : _eventProcessorCancellationTokenSource.Token
                    );

                    switch (message.OpCode)
                    {
                        case GatewayOpCode.Dispatch
                            when message.EventName is not DispatchEventNames.Resumed and not null:
                            dispatchQueue.Enqueue(message);
                            break;
                        case GatewayOpCode.Dispatch when message.EventName is DispatchEventNames.Resumed:
                            break;
                        default:
                            await ProcessMessageAsync(message, _eventProcessorCancellationTokenSource.Token);
                            break;
                    }

                    _eventProcessorCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (message.EventName is DispatchEventNames.Resumed)
                        break;
                }

                _eventProcessorCancellationTokenSource.Token.ThrowIfCancellationRequested();

                _logger.LogInformation("Resume successful, dispatching {Count} missed events", dispatchQueue.Count);

                heartbeatTask = Task.Run(() => HeartbeatLoopAsync(
                    _heartbeatInterval,
                    _eventProcessorCancellationTokenSource.Token
                ), _eventProcessorCancellationTokenSource.Token);

                while (dispatchQueue.TryDequeue(out var dispatch))
                    await HandleDispatchAsync(
                        dispatch.EventName!,
                        dispatch.Payload,
                        _eventProcessorCancellationTokenSource.Token
                    );
            }
            else
            {
                var helloPayload = GatewayMessageUtils.AsGatewayPayloadData<IHelloPayloadData>(
                    await ReceiveGatewayMessageAsync(initialMessageCancellationTokenSource.Token),
                    GatewayOpCode.Hello
                );

                _heartbeatInterval = helloPayload.HeartbeatInterval;

                _logger.LogDebug(
                    "Received Hello from discord, sending heartbeats at an interval of {Interval}",
                    _heartbeatInterval
                );

                heartbeatTask = Task.Run(() => HeartbeatLoopAsync(
                    _heartbeatInterval,
                    _eventProcessorCancellationTokenSource.Token
                ), _eventProcessorCancellationTokenSource.Token);

                await SendMessageAsync(
                    CreateIdentityMessage(),
                    _eventProcessorCancellationTokenSource.Token
                );
            }

            _logger.LogInformation("Session initialized successfully! Beginning to process events");

            while (true)
            {
                _eventProcessorCancellationTokenSource.Token.ThrowIfCancellationRequested();

                _logger.LogDebug("Waiting for message...");
                var msg = await ReceiveGatewayMessageAsync(_eventProcessorCancellationTokenSource.Token);

                await ProcessMessageAsync(
                    msg,
                    _eventProcessorCancellationTokenSource.Token
                );
            }
        }
        catch (Exception x) when (x is not OperationCanceledException and not GatewayClosedException)
        {
            await IndicateGatewayFailureAsync(x);
            throw;
        }
        catch (OperationCanceledException)
        {
            // just to ensure safety for heartbeat
            if (!_eventProcessorCancellationTokenSource.IsCancellationRequested)
                _eventProcessorCancellationTokenSource.Cancel();

            _logger.LogDebug("Event processing is shutting down (cancelled)");
        }
        catch (GatewayClosedException)
        {
            // just to ensure safety for heartbeat
            if (!_eventProcessorCancellationTokenSource.IsCancellationRequested)
                _eventProcessorCancellationTokenSource.Cancel();

            _logger.LogDebug("Event processing is shutting down (connection closed)");
        }
        catch (Exception x)
        {
            _logger.LogError(x, "Exception in event processing loop");

            if (!_eventProcessorCancellationTokenSource.IsCancellationRequested)
                _eventProcessorCancellationTokenSource.Cancel();

            if (heartbeatTask is not null)
            {
                try
                {
                    await heartbeatTask;
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
        finally
        {
            if (heartbeatTask?.IsCompleted ?? false)
                heartbeatTask.Dispose();
        }
    }

    private IGatewayMessage CreateResumeMessage(string sessionId, int sequence)
    {
        return new GatewayMessage()
        {
            OpCode = GatewayOpCode.Resume,
            Payload = new ResumePayloadData
            {
                SessionId = sessionId,
                SessionToken = Config.Token.Value,
                Sequence = sequence
            }
        };
    }

    private IGatewayMessage CreateIdentityMessage()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;

        return new GatewayMessage
        {
            OpCode = GatewayOpCode.Identify,
            Payload = new IdentityPayloadData()
            {
                Token = Config.Token.Value,
                Properties = new IdentityConnectionProperties
                {
                    Browser = $"Discord.Net {version}",
                    OS = Environment.OSVersion.Platform.ToString(),
                    Device = $"Discord.Net {version}"
                },
                Intents = (int) Config.Intents,
                Compress = Config.UsePayloadCompression,
                LargeThreshold = Config.LargeThreshold
            }
        };
    }

#pragma warning disable CA2016
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    private async Task ProcessMessageAsync(IGatewayMessage message, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        _logger.LogDebug("Processing OpCode '{Code}'", message.OpCode);

        switch (message.OpCode)
        {
            case GatewayOpCode.Dispatch when message.EventName is not null:
                await HandleDispatchAsync(message.EventName, message.Payload, token);
                break;
            case GatewayOpCode.Heartbeat:
                await _heartbeatSignal.Writer.WriteAsync(HeartbeatSignal.Requested, token);
                break;
            case GatewayOpCode.HeartbeatAck:
                await _heartbeatSignal.Writer.WriteAsync(HeartbeatSignal.ReceivedAck, token);
                break;
            case GatewayOpCode.Reconnect:
                // Don't pass 'token', it will get cancelled for the reconnect.
                await ReconnectInternalAsync(gracefulDisconnect: false);
                break;
            case GatewayOpCode.InvalidSession:
                if (message.Payload is not IInvalidSessionPayloadData invalidSessionPayload)
                    throw new UnexpectedGatewayPayloadException(typeof(InvalidSessionPayloadData), message.Payload);

                // Don't pass 'token', it will get cancelled for the reconnect.
                await ReconnectInternalAsync(
                    shouldResume: invalidSessionPayload.CanResume,
                    gracefulDisconnect: false
                );
                break;
            default:
                _logger.LogWarning("Received unknown opcode '{OpCode}'", message.OpCode.ToString("X"));
                break;
        }
    }
#pragma warning restore CA2016

    private async Task HeartbeatLoopAsync(
        int interval,
        CancellationToken token,
        bool sendFirstHeartbeatInstantly = false)
    {
        var jitter = true;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var heartbeatDelay = interval;

                if (jitter)
                {
                    heartbeatDelay = sendFirstHeartbeatInstantly
                        ? 0
                        : (int) Math.Floor(heartbeatDelay * Random.Shared.NextSingle());
                    jitter = false;
                }

                if (heartbeatDelay > 0)
                {
                    using var heartbeatWaitCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(token);
                    var heartbeatTimeoutTask = Task.Delay(heartbeatDelay, heartbeatWaitCancellationToken.Token);
                    var heartbeatSignalTask = _heartbeatSignal.Reader.ReadAsync(
                        heartbeatWaitCancellationToken.Token
                    ).AsTask();

                    // wait for either the delay or a heartbeat signal
                    // TODO: I don't like this '.AsTask' allocation
                    var triggeringTask = await Task.WhenAny(
                        heartbeatTimeoutTask,
                        heartbeatSignalTask
                    );

                    // cancel any remaining parts
                    heartbeatWaitCancellationToken.Cancel();
                    token.ThrowIfCancellationRequested();
                    
                    _logger.LogDebug(
                        "Heartbeat interrupt: {Source}",
                        triggeringTask == heartbeatTimeoutTask ? "Interval elapsed" : "Discord sent a heartbeat request"
                    );
                }

                var attempts = 0;

                while (true)
                {
                    if (attempts >= 3)
                    {
                        throw new HeartbeatUnacknowledgedException(attempts);
                    }

                    await SendMessageAsync(
                        new GatewayMessage
                        {
                            OpCode = GatewayOpCode.Heartbeat,
                            Payload = new HeartbeatPayloadData()
                            {
                                LastSequence = _sequence
                            }
                        },
                        token
                    );

                    using var heartbeatWaitCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(token);
                    heartbeatWaitCancellationToken.CancelAfter(3000);

                    try
                    {
                        var result = await _heartbeatSignal.Reader.ReadAsync(heartbeatWaitCancellationToken.Token);

                        if (result is HeartbeatSignal.ReceivedAck)
                            break;
                    }
                    catch (OperationCanceledException canceledException)
                        when (canceledException.CancellationToken == heartbeatWaitCancellationToken.Token)
                    {
                        attempts++;
                    }
                }
            }
        }
        catch (HeartbeatUnacknowledgedException ex)
        {
            _logger.LogError(ex, "Heartbeat failed");

            // don't pass the token, since we don't want to cancel the reconnect.
            // ReSharper disable once MethodSupportsCancellation
            await ReconnectInternalAsync(gracefulDisconnect: false);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Heartbeat task is shutting down (cancelled)");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Exception in heartbeat loop");
            //await IndicateGatewayFailureAsync(exception);
            throw;
        }
    }

    private async ValueTask IndicateGatewayFailureAsync(Exception exception)
    {
        // TODO:
        // invoke a 'GatewayError' event in user-code land to indicate that something went wrong

        _logger.LogError(exception, "Gateway failure occured");

        await StopGatewayConnectionAsync(false);
    }

    private async ValueTask HandleGatewayClosureAsync(GatewayReadResult result)
    {
        _logger.LogInformation("Received gateway closure: {Status}", result);

        var shouldReconnect =
            result.CloseStatusCode is >= GatewayCloseCode.UnknownError
                and <= GatewayCloseCode.SessionTimedOut
                and not GatewayCloseCode.AuthenticationFailed;

        await _connectionSemaphore.WaitAsync();

        try
        {
            await StopGatewayConnectionAsync(false);

            if (shouldReconnect) await ConnectInternalAsync();
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }
    
    private async ValueTask StopGatewayConnectionAsync(bool graceful, CancellationToken token = default)
    {
        ShardId = null;
        TotalShards = null;

        _eventProcessorCancellationTokenSource.Cancel();

        if (_eventProcessorTask is not null && graceful)
        {
            try
            {
                _logger.LogDebug("Waiting for event processor to shutdown (graceful disconnect)...");
                await _eventProcessorTask;
            }
            catch (OperationCanceledException)
            {
            }

            _logger.LogDebug("Event processor has been successfully stopped.");
        }

        if (_eventProcessorTask?.IsCompleted ?? false)
            _eventProcessorTask?.Dispose();

        if (_connection is not null)
        {
            _logger.LogDebug("Sending disconnect request to the underlying gateway connection...");
            await _connection.DisconnectAsync(token);
        }

        UnavailableGuilds.Clear();
        ProcessedUnavailableGuilds.Clear();
        
        IsConnected = false;
    }

    private async Task<IGatewayMessage> ReceiveGatewayMessageAsync(CancellationToken token = default)
    {
        if (_connection is null)
            throw new NullReferenceException("Connection was null");

        var stream = StreamManager.GetStream(nameof(ReceiveGatewayMessageAsync));

        try
        {
            _logger.LogDebug("Calling read on underlying gateway connection...");
            var result = await _connection.ReadAsync(stream, token);

            _logger.LogDebug("Read complete: {Result}", result);

            if (result.CloseStatusCode.HasValue)
            {
                await HandleGatewayClosureAsync(result);
                throw new GatewayClosedException(result);
            }

            stream.Position = 0;

            if (_compression is not null && result.Format is TransportFormat.Binary)
            {
                _logger.LogDebug("Running decompression...");

                var secondary = StreamManager.GetStream(nameof(ReceiveGatewayMessageAsync));
                await _compression.DecompressAsync(stream, secondary, token);
                await stream.DisposeAsync();
                stream = secondary;

                stream.Position = 0;
            }
            else if (result.Format != Encoding.Format)
            {
                throw new InvalidOperationException(
                    $"Unable to decode gateway message: The encoding '{Encoding.Identifier}' doesn't support the " +
                    $"message format type '{result.Format}'"
                );
            }

            _logger.LogDebug("Running decoder...");

            var start = DateTimeOffset.UtcNow.Ticks;

            var message =
                await Encoding.DecodeAsync<IGatewayMessage>(stream, token)
                ?? throw new NullReferenceException("Received a null gateway message");

            var delta = DateTimeOffset.UtcNow.Ticks - start;

            _logger.LogDebug(
                "Decoder took {Milliseconds} milliseconds ({Bytes} bytes per ms)",
                TimeSpan.FromTicks(delta),
                Math.Round(stream.Position / TimeSpan.FromTicks(delta).TotalMilliseconds, 2)
            );

            if (message.Sequence.HasValue)
            {
                lock (_sequenceSyncRoot) _sequence = message.Sequence.Value;
            }

            _logger.LogDebug("C<-S: {OpCode} {EventName} | Sequence: {Seq}", message.OpCode,
                message.EventName ?? string.Empty, _sequence);

            return message;
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }

    private async Task SendMessageAsync(IGatewayMessage message, CancellationToken token = default)
    {
        if (_connection is null)
            throw new NullReferenceException("Connection was null");

        _logger.LogDebug("Preparing to send an outbound message, entering ratelimit...");
        await WaitForOutboundRateLimitsAsync(token);

        var stream = StreamManager.GetStream(nameof(ReceiveGatewayMessageAsync));

        try
        {
            _logger.LogDebug("Encoding outbound message...");
            await Encoding.EncodeAsync(stream, message, token);

            stream.Position = 0;

            _logger.LogDebug("Sending outbound message...");
            await _connection.SendAsync(stream, Encoding.Format, token);

            _logger.LogDebug("C->S: {OpCode}", message.OpCode);

#if DEBUG
            if (message.OpCode is not GatewayOpCode.Identify)
            {
                stream.Position = 0;
                var utf8Buffer = new byte[stream.Length];
                _ = stream.Read(utf8Buffer);
                _logger.LogDebug("Payload: {Message}", System.Text.Encoding.UTF8.GetString(utf8Buffer));
            }
#endif
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }
}
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Diagnostics.CodeAnalysis;
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

    private static readonly RecyclableMemoryStreamManager _streamManager = new();

    public bool IsConnected { get; private set; }

    public int? ShardId { get; private set; }
    public int? TotalShards { get; private set; }

    private IGatewayConnection? _connection;
    private Task? _eventProcessorTask;
    private CancellationTokenSource _eventProcessorCancellationTokenSource = new();

    [MemberNotNullWhen(true, nameof(_sessionId), nameof(_resumeGatewayUrl))]
    private bool CanResume => _sessionId is not null && _resumeGatewayUrl is not null;

    private string? _resumeGatewayUrl;
    private string? _sessionId;
    private int _sequence;
    private int _heartbeatInterval;

    private readonly Channel<HeartbeatSignal> _heartbeatSignal;

    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);

    private void HandleHeartbeatSignalDropped(HeartbeatSignal signal)
    {
        // TODO: the heartbeat task has choked and we need to reconnect
        _eventProcessorCancellationTokenSource.Cancel();
    }

    private async Task StartEventProcessorAsync()
    {
        _eventProcessorCancellationTokenSource.Cancel();

        if (_eventProcessorTask is not null)
            await _eventProcessorTask;

        _eventProcessorTask?.Dispose();

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
        }

        var uriBuilder =
            new StringBuilder($"{url}?v={Config.GatewayVersion}&encoding={Encoding.Identifier}");

        if (GatewayCompression is not null)
            uriBuilder.Append($"&encoding={GatewayCompression.Identifier}");

        return new Uri(uriBuilder.ToString());
    }

    public async Task ConnectAsync(CancellationToken token = default)
    {
        await _connectionSemaphore.WaitAsync(token);

        try
        {
            if (IsConnected) return;

            await ConnectInternalAsync(token: token);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    public async ValueTask DisconnectAsync(CancellationToken token = default)
    {
        await _connectionSemaphore.WaitAsync(token);

        try
        {
            await StopGatewayConnectionAsync(token);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    public ValueTask ReconnectAsync(CancellationToken token = default)
        => ReconnectInternalAsync(token: token);

    private async ValueTask ReconnectInternalAsync(bool shouldResume = true, CancellationToken token = default)
    {
        await _connectionSemaphore.WaitAsync(token);

        try
        {
            await StopGatewayConnectionAsync(token);
            await ConnectInternalAsync(shouldResume, token);
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    private async Task ConnectInternalAsync(bool shouldResume = true, CancellationToken token = default)
    {
        _connection ??= Config.GatewayConnection.Get(this);

        var gatewayUri = await GetGatewayUriAsync(shouldResume, token);

        await _connection.ConnectAsync(gatewayUri, token);

        IsConnected = true;

        await StartEventProcessorAsync();
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
                    CreateResumeMessage(_sessionId),
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

                heartbeatTask = HeartbeatLoopAsync(
                    _heartbeatInterval,
                    _eventProcessorCancellationTokenSource.Token
                );

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

                heartbeatTask = HeartbeatLoopAsync(
                    _heartbeatInterval,
                    _eventProcessorCancellationTokenSource.Token
                );

                await SendMessageAsync(
                    CreateIdentityMessage(),
                    _eventProcessorCancellationTokenSource.Token
                );
            }

            while (true)
            {
                _eventProcessorCancellationTokenSource.Token.ThrowIfCancellationRequested();

                await ProcessMessageAsync(
                    await ReceiveGatewayMessageAsync(_eventProcessorCancellationTokenSource.Token),
                    _eventProcessorCancellationTokenSource.Token
                );
            }
        }
        catch (Exception x) when (x is not OperationCanceledException and not GatewayClosedException)
        {
            await IndicateGatewayFailureAsync(x);
            throw;
        }
        catch
        {
            if (!_eventProcessorCancellationTokenSource.IsCancellationRequested)
                _eventProcessorCancellationTokenSource.Cancel();

            if (heartbeatTask is not null)
            {
                try { await heartbeatTask; }
                catch (OperationCanceledException) { }
            }
        }
        finally
        {
            heartbeatTask?.Dispose();
        }
    }

    private IGatewayMessage CreateResumeMessage(string sessionId)
    {
        return new GatewayMessage()
        {
            OpCode = GatewayOpCode.Resume,
            Payload = new ResumePayloadData
            {
                SessionId = sessionId, SessionToken = Config.Token.Value, Sequence = _sequence
            }
        };
    }

    private IGatewayMessage CreateIdentityMessage()
    {
        return new GatewayMessage
        {
            OpCode = GatewayOpCode.Identify,
            Payload = new IdentityPayloadData()
            {
                Token = Config.Token.Value,
                Properties = new IdentityConnectionProperties
                {
                    Browser = $"Discord.Net {Environment.Version}",
                    OS = Environment.OSVersion.Platform.ToString(),
                    Device = $"Discord.Net {Environment.Version}"
                },
                Intents = (ulong)Config.Intents
            }
        };
    }

#pragma warning disable CA2016
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    private async Task ProcessMessageAsync(IGatewayMessage message, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

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
                await ReconnectAsync();
                break;
            case GatewayOpCode.InvalidSession:
                if (message.Payload is not IInvalidSessionPayloadData invalidSessionPayload)
                    throw new UnexpectedGatewayPayloadException(typeof(InvalidSessionPayloadData), message.Payload);

                // Don't pass 'token', it will get cancelled for the reconnect.
                await ReconnectInternalAsync(invalidSessionPayload.CanResume);
                break;
        }
    }
#pragma warning restore CA2016

    private async Task HeartbeatLoopAsync(int interval, CancellationToken token)
    {
        var jitter = true;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var heartbeatDelay = interval;

                if (jitter)
                {
                    heartbeatDelay = (int)Math.Floor(heartbeatDelay * Random.Shared.NextSingle());
                    jitter = false;
                }

                using (var heartbeatWaitCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(token))
                using (var heartbeatTimeoutTask = Task.Delay(heartbeatDelay, heartbeatWaitCancellationToken.Token))
                using (var heartbeatSignalTask =
                       _heartbeatSignal.Reader.ReadAsync(heartbeatWaitCancellationToken.Token).AsTask())
                {
                    // wait for either the delay or a heartbeat signal
                    // TODO: I don't like this '.AsTask' allocation
                    await Task.WhenAny(
                        heartbeatTimeoutTask,
                        heartbeatSignalTask
                    );

                    // cancel any remaining parts
                    heartbeatWaitCancellationToken.Cancel();
                }

                var attempts = 0;

                while (true)
                {
                    if (attempts == 3)
                        throw new DiscordException("Heartbeat wasn't acknowledged");

                    await SendMessageAsync(new GatewayMessage {OpCode = GatewayOpCode.Heartbeat, Sequence = _sequence},
                        token);

                    using var heartbeatWaitCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(token);
                    using var heartbeatTimeoutTask = Task.Delay(3000, heartbeatWaitCancellationToken.Token);
                    using var heartbeatSignalTask =
                        _heartbeatSignal.Reader.ReadAsync(heartbeatWaitCancellationToken.Token).AsTask();

                    var completingTask = await Task.WhenAny(heartbeatTimeoutTask, heartbeatTimeoutTask);

                    if (token.IsCancellationRequested)
                        return;

                    if (
                        completingTask == heartbeatSignalTask &&
                        heartbeatSignalTask.Result is HeartbeatSignal.ReceivedAck)
                    {
                        break;
                    }

                    attempts++;
                }
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await IndicateGatewayFailureAsync(exception);
            throw;
        }
    }

    private async ValueTask IndicateGatewayFailureAsync(Exception exception)
    {
        // TODO:
        // invoke a 'GatewayError' event in user-code land to indicate that something went wrong

        _logger.LogError(exception, "Gateway failure occured");

        await StopGatewayConnectionAsync();
    }

    private async ValueTask HandleGatewayClosureAsync(GatewayCloseStatus status)
    {
        _logger.LogInformation("Received gateway closure: {Status}", status);

        var shouldReconnect =
            status.StatusCode is >= GatewayCloseCode.UnknownError
                and <= GatewayCloseCode.SessionTimedOut
                and not GatewayCloseCode.AuthenticationFailed;

        await _connectionSemaphore.WaitAsync();

        try
        {
            await StopGatewayConnectionAsync();

            if (shouldReconnect) await ConnectInternalAsync();
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }


    private async ValueTask StopGatewayConnectionAsync(CancellationToken token = default)
    {
        ShardId = null;
        TotalShards = null;

        _eventProcessorCancellationTokenSource.Cancel();

        if (_eventProcessorTask is not null)
        {
            try { await _eventProcessorTask; }
            catch (OperationCanceledException) { }
        }

        _eventProcessorTask?.Dispose();

        if (_connection is not null)
            await _connection.DisconnectAsync(token);

        IsConnected = false;
    }

    private async Task<IGatewayMessage> ReceiveGatewayMessageAsync(CancellationToken token = default)
    {
        if (_connection is null)
            throw new NullReferenceException("Connection was null");

        var stream = _streamManager.GetStream(nameof(ReceiveGatewayMessageAsync));

        try
        {
            var result = await _connection.ReadAsync(stream, token);

            if (result.HasValue)
            {
                await HandleGatewayClosureAsync(result.Value);
                throw new GatewayClosedException(result.Value);
            }

            if (GatewayCompression is not null)
            {
                var secondary = _streamManager.GetStream(nameof(ReceiveGatewayMessageAsync));
                await GatewayCompression.DecompressAsync(stream, secondary, token);
                await stream.DisposeAsync();
                stream = secondary;
            }

            var message =
                await Encoding.DecodeAsync<IGatewayMessage>(stream, token)
                ?? throw new NullReferenceException("Received a null gateway message");

            if (message.Sequence.HasValue)
                Interlocked.Exchange(ref _sequence, message.Sequence.Value);

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

        using var stream = new MemoryStream();
        await Encoding.EncodeAsync(stream, message, token);
        await _connection.SendAsync(stream, token);
    }
}

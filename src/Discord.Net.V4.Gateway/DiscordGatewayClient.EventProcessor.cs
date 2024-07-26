using Discord.Models.Json;
using Discord.Rest;
using System.Threading.Channels;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    public bool IsConnected { get; private set; }

    private IGatewayConnection? _connection;
    private Task _eventProcessorTask;
    private CancellationTokenSource _eventProcessorCancellationTokenSource = new();
    private int _sequence;
    private TaskCompletionSource _heartbeatAckPromise = new();

    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);

    private async Task StartEventProcessorAsync()
    {
        await _eventProcessorCancellationTokenSource.CancelAsync();
        await _eventProcessorTask;

        _eventProcessorTask.Dispose();

        _eventProcessorCancellationTokenSource.Dispose();
        _eventProcessorCancellationTokenSource = new();

        _eventProcessorTask = Task.Run(EventProcessorLoopAsync);
    }

    private async ValueTask<Uri> GetGatewayUriAsync(CancellationToken token = default)
    {
        // TODO: resume
        var getGatewayResponse = await Rest.RestApiClient.ExecuteAsync(
            Routes.GetGateway,
            DefaultRequestOptions,
            token
        );

        if (getGatewayResponse is null)
            throw new NullReferenceException("get gateway was null");

        return new Uri($"{getGatewayResponse.Url}?v={Config.GatewayVersion}&encoding={Encoding.Identifier}");
    }

    public async Task ConnectAsync(CancellationToken token = default)
    {
        await _connectionSemaphore.WaitAsync(token);

        try
        {
            if (IsConnected) return;

            if (_connection is not null)
                await _connection.DisconnectAsync(token);

            _connection ??= Config.GatewayConnection(this, Config);

            var gatewayUri = await GetGatewayUriAsync(token);

            await _connection.ConnectAsync(gatewayUri, token);

            IsConnected = true;

            await StartEventProcessorAsync();
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    private async Task EventProcessorLoopAsync()
    {
        // receive hello payload
        // begin heartbeat interval
        // send identity
        // reconnection

        Task? heartbeatTask = null;
        try
        {
            using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var helloCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _eventProcessorCancellationTokenSource.Token,
                timeoutTokenSource.Token
            );

            var helloPayload = GatewayMessageUtils.AsGatewayPayloadData<IHelloPayloadData>(
                await ReceiveGatewayMessageAsync(helloCancellationTokenSource.Token),
                GatewayOpCode.Hello
            );

            heartbeatTask = HeartbeatLoopAsync(
                helloPayload.HeartbeatInterval,
                _eventProcessorCancellationTokenSource.Token
            );

            await SendMessageAsync(
                new GatewayMessage
                {
                    OpCode = GatewayOpCode.Identify,
                    Payload = new IdentityPayload()
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
                },
                _eventProcessorCancellationTokenSource.Token
            );
        }
        catch (Exception x) when (x is not OperationCanceledException)
        {
            await IndicateGatewayFailureAsync(x);
            throw;
        }
        catch
        {
            if (!_eventProcessorCancellationTokenSource.IsCancellationRequested)
                await _eventProcessorCancellationTokenSource.CancelAsync();

            // wait for heartbeat to stop
            if (heartbeatTask is not null)
                await heartbeatTask;
        }
    }

    private async Task HeartbeatLoopAsync(int interval, CancellationToken token)
    {
        var jitter = true;

        while (!token.IsCancellationRequested)
        {
            var heartbeatDelay = interval;

            if (jitter)
            {
                heartbeatDelay = (int)Math.Floor(heartbeatDelay * Random.Shared.NextSingle());
                jitter = false;
            }

            await Task.Delay(heartbeatDelay, token);

            var attempts = 0;

            while (true)
            {
                if (attempts == 3)
                    throw new DiscordException("Heartbeat wasn't acknowledged");

                await SendMessageAsync(new GatewayMessage {OpCode = GatewayOpCode.Heartbeat, Sequence = _sequence},
                    token);

                var timeout = Task.Delay(3000, token);

                // try 3 times if we don't receive a response
                var completingTask = await Task.WhenAny(timeout, _heartbeatAckPromise.Task);

                if (_eventProcessorCancellationTokenSource.IsCancellationRequested)
                    return;

                if (completingTask == _heartbeatAckPromise.Task)
                    break;

                attempts++;
            }
        }
    }

    private static ValueTask IndicateGatewayFailureAsync(Exception exception)
    {
        // TODO:
        // invoke a 'GatewayError' event in user-code land to indicate that something went wrong
        return ValueTask.CompletedTask;
    }

    private async Task<IGatewayMessage> ReceiveGatewayMessageAsync(CancellationToken token = default)
    {
        if (_connection is null)
            throw new NullReferenceException("Connection was null");

        var data = await _connection.ReadAsync(token);

        var message =
            await Encoding.DecodeAsync<IGatewayMessage>(data, token)
            ?? throw new NullReferenceException("Received a null gateway message");

        if (message.Sequence.HasValue)
            Interlocked.Exchange(ref _sequence, message.Sequence.Value);

        return message;
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

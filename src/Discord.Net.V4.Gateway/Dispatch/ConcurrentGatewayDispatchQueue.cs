using Discord.Models;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway.Events;

public sealed partial class ConcurrentGatewayDispatchQueue : IGatewayDispatchQueue
{
    private readonly DiscordGatewayClient _client;
    private readonly ILogger<ConcurrentGatewayDispatchQueue> _logger;

    [TypeFactory]
    public ConcurrentGatewayDispatchQueue(DiscordGatewayClient client)
    {
        _client = client;
        _logger = client.LoggerFactory.CreateLogger<ConcurrentGatewayDispatchQueue>();
    }

    public async ValueTask AcceptAsync(
        string eventName,
        IDispatchEvent? dispatchEvent,
        IGatewayPayloadData? payload,
        CancellationToken token)
    {
        if (dispatchEvent is null)
        {
            _logger.LogWarning(
                "Received dispatch {Event}, but no dispatch event exists for that event type",
                eventName
            );
            return;
        }

        if (dispatchEvent.RequiresPreparation)
        {
            _logger.LogDebug("Preparing dispatch event {Event}", eventName);
            dispatchEvent.Prepare();
        }

        if (!dispatchEvent.HasSubscribers)
        {
            _logger.LogDebug("Exiting early, {Event} has no subscribers", eventName);
            return;
        }

        var dispatcher = _client.GetDispatcher(eventName);

        var handlers = await dispatchEvent.GetHandlersAsync(payload, token);

        if (handlers is null)
        {
            _logger.LogDebug("Dispatch event {Event} returned no handlers", eventName);
            return;
        }

        _logger.LogInformation("Dispatching {Event}", eventName);
        await dispatcher.DispatchAsync(eventName, handlers, token);
    }
}

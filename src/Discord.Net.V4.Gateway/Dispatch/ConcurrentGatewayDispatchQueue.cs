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
        HashSet<IDispatchEvent>? dispatchEvents,
        IGatewayPayloadData? payload,
        CancellationToken token)
    {
        if (dispatchEvents is null)
        {
            _logger.LogWarning(
                "Received dispatch {Event}, but no dispatch event emitters exists for that event type",
                eventName
            );
            return;
        }

        var preparedHandlers = new List<PreparedInvocableEventHandle>();

        foreach (var dispatchEvent in dispatchEvents)
        {
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

            var handlers = await dispatchEvent.GetHandlersAsync(payload, token);

            if (handlers is null)
                continue;

            preparedHandlers.AddRange(handlers);
        }

        var dispatcher = _client.GetDispatcher(eventName);

        _logger.LogInformation(
            "Dispatching {Event} to {SubscriberCount} subscribers",
            eventName,
            preparedHandlers.Count
        );

        await dispatcher.DispatchAsync(eventName, preparedHandlers, token);
    }
}

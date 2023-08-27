using System;
namespace Discord.Gateway.EventProcessors
{
    internal abstract class EventProcessor<TPayload>
        : EventProcessor
    {
        public EventProcessor(string eventName)
            : base(eventName)
        {
            
        }

        public abstract ValueTask ProcessAsync(
            DiscordGatewayClient client,
            TPayload? payload);

        public override ValueTask ProcessAsync(DiscordGatewayClient client, object? payload)
            => ProcessAsync(client, client.Encoding.ToObject<TPayload>(payload));
    }

    internal abstract class EventProcessor
    {
        private static readonly Dictionary<string, EventProcessor> _processors;

        static EventProcessor()
        {
            _processors = typeof(EventProcessor).Assembly.GetTypes()
                .Where(x =>
                    x.BaseType is not null &&
                    x.BaseType.IsGenericType &&
                    x.BaseType.GetGenericTypeDefinition() == typeof(EventProcessor<>) &&
                    x.GetConstructor(Type.EmptyTypes) is not null
                )
                .Select(x => (EventProcessor)Activator.CreateInstance(x)!)
                .ToDictionary(x => x.EventName, x => x);
        }

        protected readonly string EventName;

        public EventProcessor(string eventName)
        {
            EventName = eventName;
        }

        public static ValueTask ProcessEventAsync(DiscordGatewayClient client, string name, object? payload)
        {
            if (!_processors.TryGetValue(name, out var processor))
                throw new KeyNotFoundException($"No processor found for the event {name}");

            return processor.ProcessAsync(client, payload);
        }

        public abstract ValueTask ProcessAsync(
            DiscordGatewayClient client,
            object? payload);
    }
}


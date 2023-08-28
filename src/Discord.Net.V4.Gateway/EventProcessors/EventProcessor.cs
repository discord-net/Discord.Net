using System;
namespace Discord.Gateway.EventProcessors
{
    internal abstract class EventProcessor<TPayload>
        : EventProcessor
    {
        public EventProcessor(params string[] eventNames)
            : base(eventNames)
        {
            
        }

        public abstract ValueTask ProcessAsync(
            DiscordGatewayClient client,
            string name,
            TPayload? payload,
            CancellationToken token);

        public override ValueTask ProcessAsync(DiscordGatewayClient client, string name, object? payload, CancellationToken token)
            => ProcessAsync(client, name, PayloadAs<TPayload>(client, payload), token);
    }

    internal abstract class EventProcessor
    {
        private static readonly Dictionary<string, EventProcessor> _processors;

        static EventProcessor()
        {
            _processors = typeof(EventProcessor).Assembly.GetTypes()
                .Where(x =>
                    x.IsAssignableTo(typeof(EventProcessor)) &&
                    !x.IsAbstract && 
                    x.GetConstructor(Type.EmptyTypes) is not null
                )
                .Select(x => (EventProcessor)Activator.CreateInstance(x)!)
                .SelectMany(x => x.EventName.Select(y => (EventName: y, Processor: x)))
                .ToDictionary(x => x.EventName, x => x.Processor);
        }

        protected readonly string[] EventName;

        public EventProcessor(params string[] eventName)
        {
            EventName = eventName;
        }

        protected static T? PayloadAs<T>(DiscordGatewayClient client, object? payload)
            => client.Encoding.ToObject<T>(payload);

        public static ValueTask ProcessEventAsync(DiscordGatewayClient client, string name, object? payload, CancellationToken token = default)
        {
            if (!_processors.TryGetValue(name, out var processor))
                throw new KeyNotFoundException($"No processor found for the event {name}");

            return processor.ProcessAsync(client, name, payload, token);
        }

        public abstract ValueTask ProcessAsync(
            DiscordGatewayClient client,
            string name,
            object? payload,
            CancellationToken token);
    }
}


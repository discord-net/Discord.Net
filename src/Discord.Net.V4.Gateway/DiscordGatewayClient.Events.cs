using System;
using System.Collections.Concurrent;

namespace Discord.Gateway
{
    public sealed partial class DiscordGatewayClient
    {
        public event GuildCreatedEvent GuildCreated
        {
            add => AddEvent(value);
            remove => RemoveEvent(value);
        }

        private readonly struct QueuedEvent
        {
            public readonly Type EventType;
            public readonly EventHandler.Handle[] Handles;

            public QueuedEvent(Type type, EventHandler.Handle[] handles)
            {
                EventType = type;
                Handles = handles;
            }
        }

        private readonly ConcurrentDictionary<Type, HashSet<EventHandler>> _events
            = new();

        private readonly ConcurrentQueue<QueuedEvent> _eventQueue = new();

        internal void QueueEvent<T>(params object?[] args)
        {
            if (!_events.TryGetValue(typeof(T), out var handlers) || handlers.Count == 0)
                return;

            var handles = handlers.Select(x => x.CreateHandle(in args))
                .ToArray();

            _eventQueue.Enqueue(new QueuedEvent(typeof(T), handles));
        }

        private void AddEvent<T>(T handler)
            where T : Delegate
        {
            _events.AddOrUpdate(
                typeof(T),
                (_, value) => new HashSet<EventHandler>() { new(handler) },
                (_, existing, value) =>
                {
                    existing.Add(new(handler));
                    return existing;
                },
                handler
            );
        }

        private void RemoveEvent<T>(T handler)
            where T : Delegate
        {
            if (!_events.TryGetValue(typeof(T), out var handlers))
                return;

            handlers.RemoveWhere(x => x.HasDelegate(handler));
        }
    }
}


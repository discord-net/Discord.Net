using System;
using System.Collections.Concurrent;

namespace Discord.Gateway
{
    public sealed partial class DiscordGatewayClient
    {
        public event GuildCreatedEvent GuildCreated
        {
            add => AddEvent(EventNames.GuildCreate, value);
            remove => RemoveEvent(EventNames.GuildCreate, value);
        }


        private readonly ConcurrentDictionary<string, HashSet<Delegate>> _events
            = new();

        internal ValueTask DispatchEvent<TArg>(string name, Func<TArg, ValueTask> _, TArg arg)
        {

        }

        private void AddEvent<T>(string name, T handler)
            where T : Delegate
        {
            _events.AddOrUpdate(
                name,
                (_, value) => new HashSet<Delegate>() { value },
                (_, existing, value) =>
                {
                    existing.Add(value);
                    return existing;
                },
                handler
            );
        }

        private void RemoveEvent<T>(string name, T handler)
            where T : Delegate
        {
            if (!_events.TryGetValue(name, out var handlers))
                return;

            handlers.Remove(handler);
        }
    }
}


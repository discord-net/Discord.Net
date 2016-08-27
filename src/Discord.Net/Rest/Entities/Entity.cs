using System;
using Discord.Rest;

namespace Discord.Rest
{
    internal abstract class Entity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        public T Id { get; }

        public abstract DiscordRestClient Discord { get; }

        internal virtual bool IsAttached => false;
        bool IEntity<T>.IsAttached => IsAttached;

        public Entity(T id)
        {
            Id = id;
        }
    }
}

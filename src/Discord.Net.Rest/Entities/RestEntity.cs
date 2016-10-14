using System;

namespace Discord.Rest
{
    public abstract class RestEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        public BaseDiscordClient Discord { get; }
        public T Id { get; }

        internal RestEntity(BaseDiscordClient discord, T id)
        {
            Discord = discord;
            Id = id;
        }
    }
}

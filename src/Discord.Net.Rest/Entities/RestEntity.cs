using System;

namespace Discord.Rest
{
    public abstract class RestEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        public T Id { get; }
        public DiscordRestClient Discord { get; }

        public RestEntity(DiscordRestClient discord, T id)
        {
            Discord = discord;
            Id = id;
        }

        IDiscordClient IEntity<T>.Discord => Discord;
    }
}

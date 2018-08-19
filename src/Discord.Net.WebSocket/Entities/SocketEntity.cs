using System;

namespace Discord.WebSocket
{
    public abstract class SocketEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        internal SocketEntity(DiscordSocketClient discord, T id)
        {
            Discord = discord;
            Id = id;
        }

        internal DiscordSocketClient Discord { get; }
        public T Id { get; }
    }
}

using System;

namespace Discord.WebSocket
{
    public abstract class SocketEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        internal DiscordSocketClient Discord { get; }
        /// <inheritdoc />
        public T Id { get; }

        internal SocketEntity(DiscordSocketClient discord, T id)
        {
            Discord = discord;
            Id = id;
        }
    }
}

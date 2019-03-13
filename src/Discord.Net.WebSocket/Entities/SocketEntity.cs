using System;

namespace Discord.WebSocket
{
    public abstract class SocketEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        public DiscordSocketClient Client { get; }
        /// <inheritdoc />
        public T Id { get; }

        internal SocketEntity(DiscordSocketClient discord, T id)
        {
            Client = discord;
            Id = id;
        }
    }
}

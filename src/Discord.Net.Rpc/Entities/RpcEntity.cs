using System;

namespace Discord.Rpc
{
    public abstract class RpcEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        public DiscordRpcClient Discord { get; }
        public T Id { get; }

        internal RpcEntity(DiscordRpcClient discord, T id)
        {
            Discord = discord;
            Id = id;
        }
    }
}

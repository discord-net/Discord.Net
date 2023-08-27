using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public abstract class GatewayEntity<T> : IEntity<T>
        where T : IEquatable<T>
    {
        internal DiscordGatewayClient Discord { get; }

        /// <inheritdoc />
        public T Id { get; }

        internal GatewayEntity(DiscordGatewayClient discord, T id)
        {
            Discord = discord;
            Id = id;
        }
    }
}

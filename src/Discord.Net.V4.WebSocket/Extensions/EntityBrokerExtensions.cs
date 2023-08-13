using Discord.WebSocket.Cache;
using Discord.WebSocket.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal static class EntityBrokerExtensions
    {
        public static IEntityProvider<TId, TEntity> ProvideSpecific<TId, TEntity>(this IEntityBroker<TId, TEntity> broker, TId id, Optional<TId> parent = default)
            where TEntity : class, ICacheableEntity<TId>
            where TId : IEquatable<TId>
            => new BrokerEntityPovider<TId, TEntity>(id, parent, broker);
    }
}

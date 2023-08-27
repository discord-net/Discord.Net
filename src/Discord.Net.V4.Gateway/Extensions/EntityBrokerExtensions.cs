using Discord.Gateway.Cache;
using Discord.Gateway.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    internal static class EntityBrokerExtensions
    {
        public delegate IEntityProvider<TId, TEntity> SpecificEntityProviderDelegate<TId, TEntity>(
            TId id, Optional<TId> parent = default)
            where TEntity : class, ICacheableEntity<TId>
            where TId : IEquatable<TId>;

        public static IEntityProvider<TId, TEntity> ProvideSpecific<TId, TEntity>(this IEntityBroker<TId, TEntity> broker, TId id, Optional<TId> parent = default)
            where TEntity : class, ICacheableEntity<TId>
            where TId : IEquatable<TId>
            => new BrokerEntityPovider<TId, TEntity>(id, parent, broker);
    }
}

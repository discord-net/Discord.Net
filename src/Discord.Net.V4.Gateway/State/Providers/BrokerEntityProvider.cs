using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State
{
    internal sealed class BrokerEntityPovider<TId, TEntity> : IEntityProvider<TId, TEntity>
        where TId : IEquatable<TId>
        where TEntity : class, ICacheableEntity<TId>
    {
        public Optional<TId> Id
            => _id;

        private readonly TId _id;
        private readonly Optional<TId> _parent;
        private readonly IEntityBroker<TId, TEntity> _broker;

        public BrokerEntityPovider(TId id, Optional<TId> parent, IEntityBroker<TId, TEntity> broker)
        {
            _id = id;
            _parent = parent;
            _broker = broker;
        }

        public async ValueTask<TEntity?> GetAsync(CancellationToken token = default(CancellationToken))
        {
            var entityHandle = await _broker.GetAsync(_id, _parent);
            return entityHandle?.Entity;
        }

        public ValueTask<IEntityHandle<TId, TEntity>?> GetHandleAsync(CancellationToken token = default(CancellationToken))
        {
            return _broker.GetAsync(_id, _parent);
        }

        public bool TryGetReferenced([NotNullWhen(true)] out TEntity? entity)
            => _broker.TryGetReferenced(_id, out entity);
    }
}

using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.State
{
    internal sealed class BrokerEntitySource<TId, TEntity> : IEntitySource<TId, TEntity>
        where TId : IEquatable<TId>
        where TEntity : class, ICacheableEntity<TId>
    {
        public Optional<TId> Id
            => _id;

        private readonly TId _id;
        private readonly Optional<TId> _parent;
        private readonly IEntityBroker<TId, TEntity> _broker;

        public BrokerEntitySource(TId id, Optional<TId> parent, IEntityBroker<TId, TEntity> broker)
        {
            _id = id;
            _parent = parent;
            _broker = broker;
        }

        public async ValueTask<TEntity?> GetAsync(CancellationToken token = default(CancellationToken))
        {
            var entityHandle = await _broker.GetAsync(_parent, _id);
            return entityHandle?.Entity;
        }

        public ValueTask<IEntityHandle<TId, TEntity>?> GetHandleAsync(CancellationToken token = default(CancellationToken))
        {
            return _broker.GetAsync(_parent, _id);
        }

        public bool TryGetReferenced([NotNullWhen(true)] out TEntity? entity)
            => _broker.TryGetReferenced(_id, out entity);
    }
}

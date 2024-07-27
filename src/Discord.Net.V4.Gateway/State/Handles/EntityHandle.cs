using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State;

internal sealed class EntityHandle<TId, TEntity> : IEntityHandle<TId, TEntity>
    where TEntity : class, ICacheableEntity<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; }
    public TEntity Entity { get; private set; }

    private readonly IEntityBroker<TId, TEntity> _broker;

    public EntityHandle(IEntityBroker<TId, TEntity> broker, TId id, TEntity entity)
    {
        Id = id;
        Entity = entity;
        _broker = broker;
    }

    public async ValueTask DisposeAsync()
    {
        // notify broker that we've released the handle

    }
}

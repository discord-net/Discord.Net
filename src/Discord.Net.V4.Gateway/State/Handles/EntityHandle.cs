using Discord.Gateway;
using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State;

internal sealed class EntityHandle<TId, TEntity, TModel> : IEntityHandle<TId, TEntity>
    where TId : IEquatable<TId>
    where TEntity : class, ICacheableEntity<TId>, IEntityOf<TModel>
    where TModel : IEntityModel<TId>
{
    public TId Id { get; }
    public TEntity Entity { get; private set; }

    private IManageableEntityBroker<TId, TEntity, TModel>? _broker;

    private bool _disposed;

    public EntityHandle(IManageableEntityBroker<TId, TEntity, TModel> broker, TId id, TEntity entity)
    {
        Id = id;
        Entity = entity;
        _broker = broker;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        Entity = null!;

        if (_broker is null) return;

        // notify broker that we've released the handle
        _broker.ReleaseHandle(this);

        _broker = null;
    }
}

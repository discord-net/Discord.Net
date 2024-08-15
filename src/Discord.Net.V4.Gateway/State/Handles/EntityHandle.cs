using Discord.Gateway;
using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State;

/// <summary>
///     An implementation of <see cref="IEntityHandle{TId,TEntity}"/>.
/// </summary>
/// <remarks>
///     The equality of this object is based purely on the <see cref="Id"/> property, ex: 2 different handle instances
///     representing the same <see cref="Id"/> are equal.
///     <br/>
///     The hashcode of this object is based on the instance and uses <see cref="RuntimeHelpers.GetHashCode(object)"/>
///     to generate a hashcode based off of the unique instance, rather than the value of the handles members.
/// </remarks>
/// <param name="id">The unique identitifer of the entity that this handle represents.</param>
/// <param name="entity">The entity instance that this handle represents.</param>
/// <param name="owningReference">The <see cref="IEntityReference{TId,TEntity}"/> that this handle is owned by.</param>
/// <typeparam name="TId">The type of the unique identifier that this handle represents.</typeparam>
/// <typeparam name="TEntity">The type of the entity that this handle represents.</typeparam>
internal sealed class EntityHandle<TId, TEntity>(
    TId id,
    TEntity entity,
    IEntityReference<TId, TEntity> owningReference
): 
    IEntityHandle<TId, TEntity>, IEquatable<EntityHandle<TId, TEntity>>
    where TId : IEquatable<TId>
    where TEntity : class, ICacheableEntity<TId>
{
    public TId Id { get; } = id;

    public TEntity Entity { get; private set; } = entity;

    private IEntityReference<TId, TEntity>? _owningReference = owningReference;

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        Entity = null!;

        if (_owningReference is null) return;

        _owningReference.ReleaseHandle(this);
        _owningReference = null;
    }

    public override string ToString()
    {
        return $"{typeof(TEntity)} '{Id}'";
    }

    public bool Equals(EntityHandle<TId, TEntity>? other)
        => other is not null && other.Id.Equals(Id);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is EntityHandle<TId, TEntity> other && Equals(other);

    public override int GetHashCode()
        => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(EntityHandle<TId, TEntity>? left, EntityHandle<TId, TEntity>? right)
        => Equals(left, right);

    public static bool operator !=(EntityHandle<TId, TEntity>? left, EntityHandle<TId, TEntity>? right)
        => !Equals(left, right);

    IEntityReference<TId> IEntityHandle<TId>.OwningReference
        => _disposed ? throw new InvalidOperationException("This handle is no longer valid") : _owningReference!;
}
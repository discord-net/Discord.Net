using Discord.WebSocket.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface IEntityHandle<TId, TEntity> : IEntityHandle
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        TId EntityId { get; }
        new TEntity? Entity { get; }

        IDisposable? CreateScopedClone(out TEntity? cloned);

        object? IEntityHandle.Entity => Entity;

        IDisposable? IEntityHandle.CreateScopedClone(out object? cloned)
            => CreateScopedClone(out cloned);

        IEntityHandle<TId, TNewEntity> Transform<TNewEntity>(Func<TEntity, TNewEntity> castFunc)
            where TNewEntity : class, IEntity<TId>
        {
            return new TransformativeHandle<TId, TEntity, TNewEntity>(this, castFunc);
        }
    }

    public interface IEntityHandle : IDisposable, IAsyncDisposable
    {
        Guid HandleId { get; }

        object? Entity { get; }

        IDisposable? CreateScopedClone(out object? cloned);

        /// <summary>
        ///     Updates the entity within the owning store.
        /// </summary>
        Task UpdateStoreAsync(CancellationToken token = default);

        /// <summary>
        ///     Refreshes the <see cref="Entity"/> property to reflect whatever
        ///     is in the store.
        /// </summary>
        Task RefreshAsync(CancellationToken token = default);

        /// <summary>
        ///     Deletes the entity from the store.
        /// </summary>
        Task DeleteAsync(CancellationToken token = default);

        ValueTask DisposeAsync(CancellationToken token);

        internal void AddNotifier(Action<IEntityHandle> onDispose);
    }
}

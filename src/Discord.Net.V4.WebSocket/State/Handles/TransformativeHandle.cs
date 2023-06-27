using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.State
{
    internal sealed class TransformativeHandle<TId, TRootEntity, TTargetEntity> : IEntityHandle<TId, TTargetEntity>
        where TRootEntity : class, IEntity<TId>
        where TTargetEntity : class, TRootEntity
        where TId : IEquatable<TId>
    {
        public TId EntityId
            => _handle.EntityId;
        public TTargetEntity? Entity
        {
            get
            {
                var entity = _handle.Entity;
                ref var target = ref VerifyDecendant(ref entity);
                return target;
            }
        }

        public Guid HandleId
            => _handle.HandleId;

        private readonly IEntityHandle<TId, TRootEntity> _handle;

        public TransformativeHandle(IEntityHandle<TId, TRootEntity> handle)
        {
            _handle = handle;
        }

        private static ref TTargetEntity? VerifyDecendant(ref TRootEntity? root)
        {
            if (root is null)
                return ref Unsafe.NullRef<TTargetEntity?>();

            if (root is not TTargetEntity)
                throw new InvalidCastException($"Expected {typeof(TTargetEntity).Name} but got {typeof(TRootEntity).Name}");

            return ref Unsafe.As<TRootEntity?, TTargetEntity?>(ref root);
        }

        public IDisposable? CreateScopedClone(out TTargetEntity? cloned)
        {
            var scope = _handle.CreateScopedClone(out TRootEntity? root);
            cloned = VerifyDecendant(ref root);
            return scope;
        }
        public Task DeleteAsync(CancellationToken token = default(CancellationToken))
            => _handle.DeleteAsync(token);
        public void Dispose()
            => _handle.Dispose();
        public ValueTask DisposeAsync(CancellationToken token)
            => _handle.DisposeAsync(token);
        public ValueTask DisposeAsync()
            => _handle.DisposeAsync();
        public Task RefreshAsync(CancellationToken token = default(CancellationToken))
            => _handle.RefreshAsync(token);
        public Task UpdateStoreAsync(CancellationToken token = default(CancellationToken))
            => _handle.UpdateStoreAsync(token);
        void IEntityHandle.AddNotifier(Action<IEntityHandle> onDispose)
            => _handle.AddNotifier(onDispose);
    }
}

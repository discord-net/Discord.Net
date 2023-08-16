using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State
{
    internal sealed class TransformativeHandle<TId, TRootEntity, TTargetEntity> : IEntityHandle<TId, TTargetEntity>
        where TRootEntity : class, IEntity<TId>
        where TTargetEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public TId EntityId
            => _handle.EntityId;

        public TTargetEntity? Entity
            => PreformCast(_handle.Entity);

        public Guid HandleId
            => _handle.HandleId;

        private readonly IEntityHandle<TId, TRootEntity> _handle;
        private readonly Func<TRootEntity, TTargetEntity> _castFunc;

        public TransformativeHandle(IEntityHandle<TId, TRootEntity> handle, Func<TRootEntity, TTargetEntity> castFunc)
        {
            _castFunc = castFunc;
            _handle = handle;
        }

        private TTargetEntity? PreformCast(TRootEntity? root)
        {
            if (root is null)
                return null;

            return _castFunc(root);
        }

        public IDisposable? CreateScopedClone(out TTargetEntity? cloned)
        {
            var scope = _handle.CreateScopedClone(out TRootEntity? root);
            cloned = PreformCast(root);
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

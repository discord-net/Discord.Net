using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.State
{
    internal sealed class TransitiveEntitySource<TId, TEntity, TTransitive> : IEntitySource<TId, TEntity>
        where TEntity : class, IEntity<TId>, TTransitive
        where TTransitive : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public Optional<TId> Id
            => _transitiveSource.Id;

        private readonly IEntitySource<TId, TTransitive> _transitiveSource;

        public TransitiveEntitySource(IEntitySource<TId, TTransitive> source)
        {
            _transitiveSource = source;
        }

        private static ref TEntity? VerifyDecendant(ref TTransitive? root)
        {
            if (root is null)
                return ref Unsafe.NullRef<TEntity?>();

            if (root is not TEntity)
                throw new InvalidCastException($"Expected {typeof(TEntity).Name} but got {typeof(TTransitive).Name}");

            return ref Unsafe.As<TTransitive?, TEntity?>(ref root);
        }

        public async ValueTask<TEntity?> GetAsync(CancellationToken token = default(CancellationToken))
        {
            var transitive = await _transitiveSource.GetAsync(token);
            return VerifyDecendant(ref transitive);
        }

        public async ValueTask<IEntityHandle<TId, TEntity>?> GetHandleAsync(CancellationToken token = default(CancellationToken))
        {
            var handle = await _transitiveSource.GetHandleAsync(token);
            return handle?.Transform<TEntity>();
        }

        public bool TryGetReferenced([NotNullWhen(true)] out TEntity? entity)
        {
            if(_transitiveSource.TryGetReferenced(out var transitive))
            {
                entity = VerifyDecendant(ref transitive)!;
                return true;
            }

            entity = null;
            return false;
        }
    }
}

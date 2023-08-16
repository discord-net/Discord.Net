using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State
{
    internal sealed class TransitiveEntityProvider<TId, TEntity, TTransitive> : IEntityProvider<TId, TEntity>
        where TEntity : class, IEntity<TId>
        where TTransitive : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public Optional<TId> Id
            => _transitiveSource.Id;

        private readonly Func<TTransitive, TEntity> _castFunc;
        private readonly IEntityProvider<TId, TTransitive> _transitiveSource;

        private TransitiveEntityProvider(IEntityProvider<TId, TTransitive> source, Func<TTransitive, TEntity> castFunc)
        {
            _castFunc = castFunc;
            _transitiveSource = source;
        }

        public static TransitiveEntityProvider<A, B, C> Create<A, B, C>(IEntityProvider<A, C> source)
            where A : IEquatable<A>
            where B : class, C
            where C : class, IEntity<A>
        {
            return new TransitiveEntityProvider<A, B, C>(source, a => a is B b ? b : throw new InvalidCastException());
        }

        public static TransitiveEntityProvider<A, B, C> Create<A, B, C>(IEntityProvider<A, C> source, Func<C, B> castFunc)
            where A : IEquatable<A>
            where B : class, IEntity<A>
            where C : class, IEntity<A>
        {
            return new TransitiveEntityProvider<A, B, C>(source, castFunc);
        }

        private TEntity? PreformCast(TTransitive? root)
        {
            if (root is null)
                return null;

            return _castFunc(root);
        }

        public async ValueTask<TEntity?> GetAsync(CancellationToken token = default(CancellationToken))
        {
            var transitive = await _transitiveSource.GetAsync(token);
            return PreformCast(transitive);
        }

        public async ValueTask<IEntityHandle<TId, TEntity>?> GetHandleAsync(CancellationToken token = default(CancellationToken))
        {
            var handle = await _transitiveSource.GetHandleAsync(token);
            return handle?.Transform(_castFunc);
        }

        public bool TryGetReferenced([NotNullWhen(true)] out TEntity? entity)
        {
            if(_transitiveSource.TryGetReferenced(out var transitive))
            {
                entity = PreformCast(transitive)!;
                return true;
            }

            entity = null;
            return false;
        }
    }
}

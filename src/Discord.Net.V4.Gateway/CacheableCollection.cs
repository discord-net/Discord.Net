using Discord.Gateway.State;
using System;
using System.Collections;
using System.Collections.Immutable;

namespace Discord.Gateway
{
    public sealed class CacheableCollection<TCacheable, TId, TGateway, TCommon> : IEntityEnumerableSource<TCommon, TId>
        where TCacheable : Cacheable<TId, TGateway>, IEntitySource<TCommon, TId>
        where TGateway : GatewayCacheableEntity<TId>, TCommon
        where TId : IEquatable<TId>
        where TCommon : class, IEntity<TId>
    {
        public async ValueTask<IReadOnlyCollection<TId>> GetIdsAsync(CancellationToken token = default)
            => (await (await _idsFactory(_parent, token)).ToArrayAsync(token)).ToImmutableArray();

        internal delegate TCacheable CacheableFactory(TId id);
        private delegate ValueTask<IAsyncEnumerable<TId>> IdsFactory(Optional<TId> parent, CancellationToken token = default);

        private sealed class WrappingEnumerator<T> : IAsyncEnumerator<T>
        {
            public T Current
                => _enumerator.Current;

            private IEnumerator<T> _enumerator;

            public WrappingEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return ValueTask.CompletedTask;
            }
            public ValueTask<bool> MoveNextAsync()
            {
                return ValueTask.FromResult(_enumerator.MoveNext());
            }
        }
        private sealed class Enumerator : IAsyncEnumerator<TCacheable>
        {
            public TCacheable Current { get; private set; }

            private IAsyncEnumerator<TId>? _ids;
            private readonly IdsFactory _idsFactory;
            private readonly CacheableFactory _factory;
            private readonly Optional<TId> _parent;
            private readonly CancellationToken _token;
            private readonly Func<CancellationToken, ValueTask>? _cleanTask;

            public Enumerator(
                Optional<TId> parent, IdsFactory idsFactory,
                CacheableFactory factory,
                Func<CancellationToken, ValueTask>? cleanTask,
                CancellationToken token)
            {
                _cleanTask = cleanTask;
                _token = token;
                _parent = parent;
                _idsFactory = idsFactory;
                _factory = factory;
                Current = null!;
            }

            public async ValueTask DisposeAsync()
            {
                if(_ids is not null)
                {
                    await _ids.DisposeAsync();
                }


                if(_cleanTask is not null)
                {
                    await _cleanTask(_token);

                }
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                _ids ??= (await _idsFactory(_parent)).GetAsyncEnumerator(_token);

                if (!await _ids.MoveNextAsync())
                    return false;

                _token.ThrowIfCancellationRequested();

                Current = _factory(_ids.Current);
                return true;
            }
        }

        private readonly CacheableFactory _factory;
        private readonly IdsFactory _idsFactory;
        private readonly Func<CancellationToken, ValueTask>? _cleanupTask;
        private readonly Optional<TId> _parent;
        private readonly IEntityBroker<TId, TGateway>? _broker;

        private readonly Func<IEnumerable<TId>>? _syncIds;

        internal CacheableCollection(IEntityBroker<TId, TGateway> broker, CacheableFactory factory)
            : this(Optional<TId>.Unspecified, broker.GetAllIdsAsync, factory, broker: broker) { }

        internal CacheableCollection(Func<IEnumerable<TId>> idsSupplier, CacheableFactory factory)
            : this(
                  Optional<TId>.Unspecified,
                  (parent, _) => ValueTask.FromResult(AsyncEnumerable.Create(token => new WrappingEnumerator<TId>(idsSupplier().GetEnumerator()))),
                  factory
              )
        {
            _syncIds = idsSupplier;
        }

        internal CacheableCollection(TId parent, IEntityBroker<TId, TGateway> broker, CacheableFactory factory)
            : this(parent, broker.GetAllIdsAsync, factory, broker: broker) { }

        internal CacheableCollection(TId parent, Func<IEnumerable<TId>> idsSupplier, CacheableFactory factory)
            : this(
                  parent,
                  (parent, _) => ValueTask.FromResult(AsyncEnumerable.Create(token => new WrappingEnumerator<TId>(idsSupplier().GetEnumerator()))),
                  factory
              )
        {
            _syncIds = idsSupplier;
        }

        private CacheableCollection(
            Optional<TId> parent, IdsFactory idsFactory,
            CacheableFactory factory,
            Func<CancellationToken, ValueTask>? cleanTask = null,
            IEntityBroker<TId, TGateway>? broker = null)
        {
            _parent = parent;
            _factory = factory;
            _idsFactory = idsFactory;
            _cleanupTask = cleanTask;
            _broker = broker;
        }

        public async ValueTask<IReadOnlyCollection<TCommon>> FlattenAsync(RequestOptions? options = null, CancellationToken token = default)
        {
            // if we can use the `GetAllAsync()` method
            if(_broker is not null)
            {
                var enumerable = await _broker.GetAllAsync(_parent, token);

                return (await enumerable.Select(x => x.Entity).ToArrayAsync(token)).ToImmutableArray();
            }

            // use path:
            // - IEntityStore.GetAllIdsAsync()
            // - iter ->
            //   - IEntityStore.GetAsync(iter.Id)
            return (await this.SelectAwait(x => x.LoadAsync(options, token)).ToArrayAsync(token)).ToImmutableArray();
        }

        public IAsyncEnumerator<TCacheable> GetAsyncEnumerator(CancellationToken token = default)
        {
            return new Enumerator(_parent, _idsFactory, _factory, _cleanupTask, token);
        }

        async ValueTask<IReadOnlyCollection<TCommon>> IEntityEnumerableSource<TCommon, TId>.FlattenAsync(RequestOptions? option, CancellationToken token)
            => await FlattenAsync(option, token);

        IAsyncEnumerator<IEntitySource<TCommon, TId>> IAsyncEnumerable<IEntitySource<TCommon, TId>>.GetAsyncEnumerator(CancellationToken cancellationToken)
            => GetAsyncEnumerator(cancellationToken);
    }
}


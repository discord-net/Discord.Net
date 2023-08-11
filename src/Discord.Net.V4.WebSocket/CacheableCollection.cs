using Discord.WebSocket.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Immutable;

namespace Discord.WebSocket
{
    public sealed class CacheableCollection<TCacheable, TId, TGateway> : IAsyncEnumerable<TCacheable>
        where TCacheable : Cacheable<TId, TGateway>
        where TGateway : SocketCacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        public async ValueTask<IReadOnlyCollection<TId>> GetIdsAsync()
            => (await (await _idsFactory(_parent)).ToArrayAsync()).ToImmutableArray();

        internal delegate TCacheable CacheableFactory(TId id);
        private delegate ValueTask<IAsyncEnumerable<TId>> IdsFactory(Optional<TId> parent);

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
                CacheableFactory factory, CancellationToken token,
                Func<CancellationToken, ValueTask>? cleanTask)
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

        internal CacheableCollection(IEntityBroker<TId, TGateway> broker, CacheableFactory factory)
            : this(Optional<TId>.Unspecified, broker.GetAllIdsAsync, factory) { }

        internal CacheableCollection(Func<IEnumerable<TId>> idsSupplier, CacheableFactory factory)
            : this(
                  Optional<TId>.Unspecified,
                  parent => ValueTask.FromResult(AsyncEnumerable.Create(token => new WrappingEnumerator<TId>(idsSupplier().GetEnumerator()))),
                  factory
              )
        { }

        internal CacheableCollection(TId parent, IEntityBroker<TId, TGateway> broker, CacheableFactory factory)
            : this(parent, broker.GetAllIdsAsync, factory) { }

        internal CacheableCollection(TId parent, Func<IEnumerable<TId>> idsSupplier, CacheableFactory factory)
            : this(
                  parent,
                  parent => ValueTask.FromResult(AsyncEnumerable.Create(token => new WrappingEnumerator<TId>(idsSupplier().GetEnumerator()))),
                  factory
              )
        { }

        private CacheableCollection(
            Optional<TId> parent, IdsFactory idsFactory,
            CacheableFactory factory, Func<CancellationToken, ValueTask>? cleanTask = null)
        {
            _parent = parent;
            _factory = factory;
            _idsFactory = idsFactory;
            _cleanupTask = cleanTask;
        }

        public IAsyncEnumerator<TCacheable> GetAsyncEnumerator(CancellationToken token = default)
        {
            return new Enumerator(_parent, _idsFactory, _factory, token, _cleanupTask);
        }
    }
}


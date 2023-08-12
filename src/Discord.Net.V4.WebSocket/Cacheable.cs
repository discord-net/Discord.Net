using Discord.WebSocket.Cache;
using Discord.WebSocket.State;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public class Cacheable<TId, TGateway, TRest, TCommon> : Cacheable<TId, TGateway>
        where TGateway : SocketCacheableEntity<TId>, TCommon
        where TRest : IEntity<TId>, TCommon // TODO: RestEntity<TId>
        where TCommon : IEntity<TId>
        where TId : IEquatable<TId>
    {
        internal Cacheable(Optional<TId> id, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : base(id, client, source)
        {
        }

        internal Cacheable(Optional<TId> id, Optional<TId> parent, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : base(id, parent, client, source)
        {

        }

        internal Cacheable(Func<Optional<TId>> idFunc, DiscordSocketClient client, CacheableSourceFactory source)
            : base(idFunc, () => default, client, source)
        {
        }

        internal Cacheable(Func<Optional<TId>> idFunc, Func<Optional<TId>> parentIdFunc, DiscordSocketClient client, CacheableSourceFactory source)
            : base(idFunc, parentIdFunc, client, source)
        {
        }

        public ValueTask<TRest> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
        {
            // TODO: fetch from rest
            return default!;
        }

        public async ValueTask<TCommon> GetOrFetchAsync(RequestOptions? options = null, CancellationToken token = default)
        {
            TCommon? entity = await GetAsync(token);

            if (entity is not null)
                return entity;

            return await FetchAsync(options, token);
        }
    }

    public class Cacheable<TId, TGateway>
        where TGateway : SocketCacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        internal delegate IEntitySource<TId, TGateway>? CacheableSourceFactory(TId? id, Optional<TId> parent);

        public Optional<TId> Id
            => _idFunc();

        public bool IsAvailable
            => _idFunc().IsSpecified;

        public TGateway? Value
        {
            get
            {
                var id = _idFunc();

                if (!id.IsSpecified)
                    return null;

                var source = _sourceFactory(id.Value, _parentIdFunc());

                if (source is null)
                    return null;

                return source.TryGetReferenced(out var value) ? value : null;
            }
        }

        protected readonly DiscordSocketClient Client;

        private readonly Func<Optional<TId>> _idFunc;
        private readonly Func<Optional<TId>> _parentIdFunc;
        private readonly CacheableSourceFactory _sourceFactory;

        internal Cacheable(Optional<TId> id, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : this(id, default, client, source)
        { }

        internal Cacheable(Optional<TId> id, Optional<TId> parent, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : this(() => id, () => parent, client, (_, __) => source)
        { }

        internal Cacheable(Func<Optional<TId>> idFunc, Func<Optional<TId>> parentFunc, DiscordSocketClient client, CacheableSourceFactory cacheableSource)
        {
            _idFunc = idFunc;
            _parentIdFunc = parentFunc;
            Client = client;
            _sourceFactory = cacheableSource;
        }

        public virtual ValueTask<IEntityHandle<TId, TGateway>?> GetHandleAsync(CancellationToken token = default)
        {
            var id = _idFunc();

            if (!id.IsSpecified)
                return ValueTask.FromResult<IEntityHandle<TId, TGateway>?>(null);

            return _sourceFactory(id.Value, _parentIdFunc())?.GetHandleAsync(token) ?? ValueTask.FromResult<IEntityHandle<TId, TGateway>?>(null);
        }

        public virtual async ValueTask<TGateway?> GetAsync(CancellationToken token = default)
        {
            if (!IsAvailable)
                return null;

            // lifetime for the entity extends to the callee
            using var handle = await GetHandleAsync(token);

            if (handle is not null)
            {
                return handle.Entity;
            }

            return null;
        }
    }
}

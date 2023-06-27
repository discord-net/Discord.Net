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
        internal Cacheable(TId id, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : base(id, client, source)
        {
        }

        internal Cacheable(TId? parent, TId id, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : base(parent, id, client, source)
        {
        }

        public ValueTask<TRest> FetchAsync()
        {
            // TODO: fetch from rest
            return default!;
        }

        public async ValueTask<TCommon> GetOrFetchAsync()
        {
            TCommon? entity = await GetAsync();

            if (entity is not null)
                return entity;

            return await FetchAsync();
        }
    }

    public class Cacheable<TId, TGateway>
        where TGateway : SocketCacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        public TId? Id
            => _id;

        [MemberNotNullWhen(true, nameof(_id), nameof(Id))]
        public bool IsAvailable
            => _id is not null;

        public TGateway? Value
           => _id != null && _source.TryGetReferenced(out var value) ? value : null;

        protected readonly DiscordSocketClient Client;

        private readonly TId? _id;
        private readonly TId? _parent;
        private readonly IEntitySource<TId, TGateway> _source;

        internal Cacheable(TId id, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
            : this(default, id, client, source)
        { }

        internal Cacheable(TId? parent, TId id, DiscordSocketClient client, IEntitySource<TId, TGateway> source)
        {
            Client = client;
            _parent = parent;
            _id = id;
            _source = source;
        }

        public virtual ValueTask<IEntityHandle<TId, TGateway>?> GetHandleAsync()
           => IsAvailable ? _source.GetHandleAsync() : ValueTask.FromResult<IEntityHandle<TId, TGateway>?>(null);

        public virtual async ValueTask<TGateway?> GetAsync()
        {
            if (!IsAvailable)
                return null;

            // lifetime for the entity extends to the callee
            using var handle = await _source.GetHandleAsync();

            if (handle is not null)
            {
                return handle.Entity;
            }

            return null;
        }
    }
}

using Discord.WebSocket.Cache;
using Discord.WebSocket.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public sealed class Cacheable<TId, TGateway, TRest, TCommon>
        where TGateway : SocketCacheableEntity<TId>, TCommon
        where TRest : IEntity<TId>, TCommon // TODO: RestEntity<TId>
        where TCommon : IEntity<TId>
        where TId : IEquatable<TId>
    {
        public TGateway? Value
           => _broker.TryGetReferenced(_id, out var value) ? value : null;

        private readonly TId _id;
        private readonly TId? _parent;
        private readonly DiscordSocketClient _client;
        private readonly IEntityBroker<TId, TGateway> _broker;

        internal Cacheable(TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
            : this(default, id, client, broker)
        { }

        internal Cacheable(TId? parent, TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
        {
            _parent = parent;
            _id = id;
            _client = client;
            _broker = broker;
        }

        public ValueTask<IEntityHandle<TId, TGateway>?> GetHandleAsync()
            => _broker.GetAsync(_parent, _id);

        public async ValueTask<TGateway?> GetAsync()
        {
            // lifetime for the entity extends the callee
            using var handle = await _broker.GetAsync(_parent, _id);

            if(handle is not null)
            {
                return handle.Entity;
            }

            return null;
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

    public sealed class Cacheable<TId, TGateway>
        where TGateway : SocketCacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        public TGateway? Value
            => _broker.TryGetReferenced(_id, out var value) ? value : null;

        private readonly TId _id;
        private readonly TId? _parent;
        private readonly DiscordSocketClient _client;
        private readonly IEntityBroker<TId, TGateway> _broker;

        internal Cacheable(TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
            : this(default, id, client, broker)
        { }

        internal Cacheable(TId? parent, TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
        {
            _parent = parent;
            _id = id;
            _client = client;
            _broker = broker;
        }

        public ValueTask<IEntityHandle<TId, TGateway>?> GetHandleAsync()
           => _broker.GetAsync(_parent, _id);

        public async ValueTask<TGateway?> GetAsync()
        {
            // lifetime for the entity extends the callee
            using var handle = await _broker.GetAsync(_parent, _id);

            if (handle is not null)
            {
                return handle.Entity;
            }

            return null;
        }
    }
}

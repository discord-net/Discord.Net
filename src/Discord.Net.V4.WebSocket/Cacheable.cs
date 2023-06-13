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
    public sealed class Cacheable<TId, TGateway, TRest, TCommon> : Cacheable<TId, TGateway>
        where TGateway : SocketCacheableEntity<TId>, TCommon
        where TRest : IEntity<TId>, TCommon // TODO: RestEntity<TId>
        where TCommon : IEntity<TId>
        where TId : IEquatable<TId>
    {
        internal Cacheable(TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
            : base(id, client, broker)
        {
        }

        internal Cacheable(TId? parent, TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
            : base(parent, id, client, broker)
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
        {
            get => _id;
            internal set => _id = value;
        }

        [MemberNotNullWhen(true, nameof(_id), nameof(Id))]
        public bool IsAvailable
            => _id is not null;

        public TGateway? Value
           => _id != null && _broker.TryGetReferenced(_id, out var value) ? value : null;

        protected readonly DiscordSocketClient Client;

        private TId? _id;
        private readonly TId? _parent;
        private readonly IEntityBroker<TId, TGateway> _broker;

        internal Cacheable(TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
            : this(default, id, client, broker)
        { }

        internal Cacheable(TId? parent, TId id, DiscordSocketClient client, IEntityBroker<TId, TGateway> broker)
        {
            Client = client;
            _parent = parent;
            _id = id;
            _broker = broker;
        }

        public ValueTask<IEntityHandle<TId, TGateway>?> GetHandleAsync()
           => IsAvailable ? _broker.GetAsync(_parent, _id) : ValueTask.FromResult<IEntityHandle<TId, TGateway>?>(null);

        public async ValueTask<TGateway?> GetAsync()
        {
            if (!IsAvailable)
                return null;

            // lifetime for the entity extends to the callee
            using var handle = await _broker.GetAsync(_parent, _id);

            if (handle is not null)
            {
                return handle.Entity;
            }

            return null;
        }
    }
}

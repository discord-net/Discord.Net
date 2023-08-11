using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;

namespace Discord.WebSocket
{
    public static class CacheableExtensions
    {
        public static async ValueTask<IReadOnlyCollection<TCommon>> GetOrFetchAllAsync
            <TId, TGateway, TRest, TCommon>
        (
            this CacheableCollection<Cacheable<TId, TGateway, TRest, TCommon>, TId, TGateway, TRest, TCommon> cacheable,
            CancellationToken token = default
        )
            where TGateway : SocketCacheableEntity<TId>, TCommon
            where TRest : IEntity<TId>, TCommon // TODO: RestEntity<TId>
            where TCommon : IEntity<TId>
            where TId : IEquatable<TId>
        {
            return (await cacheable.SelectAwait(x => x.GetOrFetchAsync()).ToArrayAsync(token)).ToImmutableArray();
        }

        public static async ValueTask<IReadOnlyCollection<TGateway?>> GetAllAsync
            <TId, TGateway, TRest, TCommon>
        (
            this CacheableCollection<Cacheable<TId, TGateway, TRest, TCommon>, TId, TGateway, TRest, TCommon> cacheable,
            CancellationToken token = default
        )
            where TGateway : SocketCacheableEntity<TId>, TCommon
            where TRest : IEntity<TId>, TCommon // TODO: RestEntity<TId>
            where TCommon : IEntity<TId>
            where TId : IEquatable<TId>
        {
            return (await cacheable.SelectAwait(x => x.GetAsync()).ToArrayAsync(token)).ToImmutableArray();
        }

        public static async ValueTask<IReadOnlyCollection<TRest>> FetchAllAsync
            <TId, TGateway, TRest, TCommon>
        (
            this CacheableCollection<Cacheable<TId, TGateway, TRest, TCommon>, TId, TGateway, TRest, TCommon> cacheable,
            CancellationToken token = default
        )
            where TGateway : SocketCacheableEntity<TId>, TCommon
            where TRest : IEntity<TId>, TCommon // TODO: RestEntity<TId>
            where TCommon : IEntity<TId>
            where TId : IEquatable<TId>
        {
            return (await cacheable.SelectAwait(x => x.FetchAsync()).ToArrayAsync(token)).ToImmutableArray();
        }
    }
}


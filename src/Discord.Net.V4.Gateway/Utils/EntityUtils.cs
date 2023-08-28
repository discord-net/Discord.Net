using Discord.API;
using Discord.Gateway.Cache;
using Discord.Gateway.State;
using System;

namespace Discord.Gateway
{
    internal static class EntityUtils
    {
        public static Cacheable<TId, TEntity, TRest, TCommon>? UpdateCacheableFrom<TId, TEntity, TRest, TCommon>(
            DiscordGatewayClient client,
            Cacheable<TId, TEntity, TRest, TCommon>? existing,
            IEntityBroker<TId, TEntity> broker,
            TId modelId,
            Optional<TId> parentId,
            CacheOperation operation,
            bool isOwned)
            where TId : struct, IEquatable<TId>
            where TEntity : GatewayCacheableEntity<TId>, TCommon
            where TRest : class, IEntity<TId>, TCommon // TODO: RestEntity<TId>
            where TCommon : class, IEntity<TId>
        {
            if (existing is not null && operation is CacheOperation.Update && !isOwned)
            {
                return null;
            }

            // the model isn't for us
            if (!isOwned)
                return existing;

            // if our instance cacheable is null and the models channel id points to us.
            if (existing is null && operation is CacheOperation.Create or CacheOperation.Update)
            {
                return new Cacheable<TId, TEntity, TRest, TCommon>(modelId, parentId, client, broker.ProvideSpecific(modelId, parentId));
            }
            // if the operation was a delete and we still hold the instance
            else if (operation is CacheOperation.Delete && existing is not null)
            {
                return null;
            }

            return existing;
        }

        public static Cacheable<TId, TEntity>? UpdateCacheableFrom<TId, TEntity>(
            TEntity entity,
            Cacheable<TId, TEntity>? existing,
            IEntityBroker<TId, TEntity> broker,
            TId modelId,
            Optional<TId> parentId,
            CacheOperation operation,
            bool isOwned)
            where TId : struct, IEquatable<TId>
            where TEntity : GatewayCacheableEntity<TId>
        {
            if (existing is not null && operation is CacheOperation.Update && !isOwned)
            {
                return null;
            }

            // the model isn't for us
            if (!isOwned)
                return existing;

            // if our instance cacheable is null and the models channel id points to us.
            if (existing is null && operation is CacheOperation.Create or CacheOperation.Update)
            {
                return new Cacheable<TId, TEntity>(modelId, parentId, entity.Discord, broker.ProvideSpecific(modelId, parentId));
            }
            // if the operation was a delete and we still hold the instance
            else if (operation is CacheOperation.Delete && existing is not null)
            {
                return null;
            }

            return existing;
        }

        public static Cacheable<TId, TEntity, TRest, TCommon>? UpdateCacheableFrom<TId, TEntity, TRest, TCommon>(
            DiscordGatewayClient client,
            Cacheable<TId, TEntity, TRest, TCommon>? existing,
            IEntityBroker<TId, TEntity> broker, TId? id, Optional<TId> parent = default)
            where TId : struct, IEquatable<TId>
            where TEntity : class, ICacheableEntity<TId>, TCommon
            where TRest : class, IEntity<TId>, TCommon // TODO: RestEntity<TId>
            where TCommon : class, IEntity<TId>
        {
            if ((existing is not null && id.HasValue && id.Value.Equals(existing.Id)) || (existing is null && id.HasValue))
            {
                return new Cacheable<TId, TEntity, TRest, TCommon>(id.Value, parent, client, broker.ProvideSpecific(id.Value, parent));
            }

            if (existing is not null && !id.HasValue)
                return null;

            return existing;
        }

        public static Cacheable<TId, TEntity>? UpdateCacheableFrom<TId, TEntity>(
            DiscordGatewayClient client,
            Cacheable<TId, TEntity>? existing,
            IEntityBroker<TId, TEntity> broker, TId? id, Optional<TId> parent = default)
            where TId : struct, IEquatable<TId>
            where TEntity : class, ICacheableEntity<TId>
        {
            if((existing is not null && id.HasValue && id.Value.Equals(existing.Id)) || (existing is null && id.HasValue))
            {
                return new Cacheable<TId, TEntity>(id.Value, parent, client, broker.ProvideSpecific(id.Value, parent));
            }

            if (existing is not null && !id.HasValue)
                return null;

            return existing;
        }

        public static Cacheable<TId, TEntity, TRest, TCommon>? CreateCacheableFrom<TId, TEntity, TRest, TCommon>(
            DiscordGatewayClient client,
            Cacheable<TId, TEntity, TRest, TCommon>? _, // template
            IEntityBroker<TId, TEntity> broker,
            TId? id, Optional<TId> parent = default)
            where TId : struct, IEquatable<TId>
            where TEntity : class, ICacheableEntity<TId>, TCommon
            where TRest : class, IEntity<TId>, TCommon // TODO: RestEntity<TId>
            where TCommon : class, IEntity<TId>
        {
            if (!id.HasValue)
                return null;

            return new Cacheable<TId, TEntity, TRest, TCommon>(id.Value, parent, client, broker.ProvideSpecific(id.Value, parent));
        }

        public static Cacheable<TId, TEntity>? CreateCacheableFrom<TId, TEntity>(
            DiscordGatewayClient client,
            Cacheable<TId, TEntity>? _, // template
            IEntityBroker<TId, TEntity> broker,
            TId? id, Optional<TId> parent = default)
            where TId : struct, IEquatable<TId>
            where TEntity : class, ICacheableEntity<TId>
        {
            if (!id.HasValue)
                return null;

            return new Cacheable<TId, TEntity>(id.Value, parent, client, broker.ProvideSpecific(id.Value, parent));
        }
    }
}


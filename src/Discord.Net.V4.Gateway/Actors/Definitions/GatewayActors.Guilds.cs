using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

using GuildsPager = GatewayPagedIndexableActor<GatewayGuildActor, ulong, GatewayGuild, RestPartialGuild,
    PageUserGuildsParams, IEnumerable<IPartialGuildModel>>;

using BansPager = GatewayPagedIndexableActor<GatewayBanActor, ulong, GatewayBan, PageGuildBansParams, IEnumerable<IBanModel>>;

internal static partial class GatewayActors
{
    public static GatewayEnumerableIndexableActor<
        TActor,
        TId,
        TEntity,
        TRestEntity,
        TCoreEntity,
        TModel
    > GuildRelatedEntity<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Ignore] TRestEntity,
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface, RequireResolve]
        TCoreEntity,
        TModel
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path
    )
        where TActor :
        class,
        IFactory<TActor, DiscordGatewayClient, GuildIdentity, IIdentifiable<TId, TEntity, TActor, TModel>>,
        IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>,
        IStoreProvider<TId, TModel>
        where TId : IEquatable<TId>
        where TEntity :
        GatewayEntity<TId>,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient>
        where TCoreEntity : class, IEntity<TId>, IFetchableOfMany<TId, TModel>
        where TModel : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCoreEntity, TModel>(
            client,
            id => TActor.Factory(client, guild, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            model => TRestEntity.Construct(
                client.Rest,
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id),
                model
            ),
            path,
            TCoreEntity.FetchManyRoute(path)
        );
    }

    public static GatewayEnumerableIndexableActor<
        TActor,
        TId,
        TEntity,
        TRestEntity,
        TCoreEntity,
        TModel,
        TApi
    > GuildRelatedEntity<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Ignore] TRestEntity,
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface] [RequireResolve, Shrink]
        TCoreEntity,
        TModel,
        [Not(nameof(TModel))] TApi
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path
    )
        where TActor :
        class,
        IFactory<TActor, DiscordGatewayClient, GuildIdentity, IIdentifiable<TId, TEntity, TActor, TModel>>,
        IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>,
        IStoreProvider<TId, TModel>
        where TId : IEquatable<TId>
        where TEntity :
        GatewayEntity<TId>,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient>
        where TCoreEntity : class, IEntity<TId>, IFetchableOfMany<TId, TApi>
        where TModel : class, TApi
        where TApi : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCoreEntity, TModel, TApi>(
            client,
            id => TActor.Factory(client, guild, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            model => TRestEntity.Construct(
                client.Rest,
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id),
                model
            ),
            path,
            TCoreEntity.FetchManyRoute(path)
        );
    }

    public static GatewayEnumerableIndexableActor<
        TActor,
        TId,
        TEntity,
        TRestEntity,
        TCoreEntity,
        TModel,
        TApi
    > GuildRelatedEntity<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Ignore] TRestEntity,
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface] [RequireResolve, Shrink]
        TCoreEntity,
        TModel,
        [Not(nameof(TModel))] TApi,
        TContext
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path,
        Func<GuildIdentity, TContext> context
    )
        where TActor :
        class,
        IFactory<TActor, DiscordGatewayClient, GuildIdentity, IIdentifiable<TId, TEntity, TActor, TModel>>,
        IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>,
        IStoreProvider<TId, TModel>
        where TId : IEquatable<TId>
        where TEntity :
        GatewayEntity<TId>,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, TContext, DiscordRestClient>
        where TCoreEntity : class, IEntity<TId>, IFetchableOfMany<TId, TApi>
        where TModel : class, TApi
        where TApi : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCoreEntity, TModel, TApi>(
            client,
            id => TActor.Factory(client, guild, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            model => TRestEntity.Construct(
                client.Rest,
                context(guild),
                model
            ),
            path,
            TCoreEntity.FetchManyRoute(path)
        );
    }

    public static BansPager PageBans(DiscordGatewayClient client, GuildIdentity guild)
    {
        return new BansPager(
            client,
            banId => new(client, guild, BanIdentity.Of(banId)),
            args => Routes.GetGuildBans(guild.Id, args.PageSize, args.Before?.Id, args.After?.Id),
            (_, models) => models.Select(x => RestBan.Construct(client.Rest, guild, x)),
        );
    }

    public static GuildsPager PageGuilds(DiscordGatewayClient client)
    {
        return new GuildsPager(
            client,
            id => new GatewayGuildActor(client, GuildIdentity.Of(id)),
            pageParams =>
                Routes.GetCurrentUserGuilds(pageParams.Before?.Id, pageParams.After?.Id, pageParams.PageSize, true),
            (_, models) => models.Select(x => RestPartialGuild.Construct(client.Rest, x)),
            (_, models, args) =>
            {
                ulong? nextId;

                if (args.After.HasValue)
                {
                    nextId = models?.MaxBy(x => x.Id)?.Id;

                    if (!nextId.HasValue)
                        return null;

                    return Routes.GetCurrentUserGuilds(
                        limit: args.PageSize,
                        after: nextId
                    );
                }

                nextId = models.MinBy(x => x.Id)?.Id;

                if (!nextId.HasValue)
                    return null;

                return Routes.GetCurrentUserGuilds(
                    limit: args.PageSize,
                    before: nextId
                );
            }
        );
    }
}

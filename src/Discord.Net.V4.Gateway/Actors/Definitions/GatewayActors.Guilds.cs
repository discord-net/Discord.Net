using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

internal static partial class GatewayActors
{
    public static GatewayEnumerableIndexableActor<
        TTrait,
        TId,
        TEntity,
        TRestEntity,
        TCoreEntity,
        TModel
    > GuildRelatedTrait<
        [TransitiveFill] TTrait,
        TRootActor,
        TId,
        TEntity,
        [Ignore] TRestEntity,
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface, RequireResolve]
        TCoreEntity,
        TModel
    >(
        Template<TTrait> template,
        Template<TRootActor> rootTemplate,
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path,
        Func<TModel, bool> filter
    )
        where TRootActor :
        class,
        IGatewayCachedActor<TId, TEntity, TModel>,
        IFactory<
            TRootActor,
            DiscordGatewayClient,
            GuildIdentity,
            IIdentifiable<TId, TEntity, TRootActor, TModel>
        >
        where TTrait :
        class,
        IFactory<
            TTrait,
            DiscordGatewayClient,
            TRootActor,
            IIdentifiable<TId, TCoreEntity, TTrait, TModel>
        >,
        IGatewayCachedActor<TId, TEntity, TModel>,
        IActorTrait<TId, TCoreEntity>
        where TId : IEquatable<TId>
        where TEntity :
        GatewayEntity<TId>,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreInfoProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        IContextConstructable<TRestEntity, TModel, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient>
        where TCoreEntity : class, IEntity<TId, TModel>, IFetchableOfMany<TId, TModel>
        where TModel : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TTrait, TId, TEntity, TRestEntity, TCoreEntity, TModel>(
            client,
            id => TTrait.Factory(
                client,
                TRootActor.Factory(
                    client,
                    guild,
                    IIdentifiable<TId, TEntity, TRootActor, TModel>.Of(id)
                ),
                IIdentifiable<TId, TCoreEntity, TTrait, TModel>.Of(id)
            ),
            model => TRestEntity.Construct(
                client.Rest,
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id),
                model
            ),
            path,
            TCoreEntity.FetchManyRoute(path),
            models => models.Where(filter)
        );
    }

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
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface, RequireResolve, Shrink]
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
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient>
        where TCoreEntity : class, IEntity<TId, TModel>, IFetchableOfMany<TId, TModel>
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
        IEnumerable<TApi>
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
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient>
        where TCoreEntity : class, IEntity<TId, TModel>, IFetchableOfMany<TId, TApi>
        where TModel : class, TApi
        where TApi : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCoreEntity, TModel,
            IEnumerable<TApi>>(
            client,
            id => TActor.Factory(client, guild, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            model => TRestEntity.Construct(
                client.Rest,
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id),
                model
            ),
            path,
            TCoreEntity.FetchManyRoute(path),
            api => api.OfType<TModel>()
        );
    }

    public static GatewayEnumerableIndexableActor<
        TActor,
        TId,
        TEntity,
        TRestEntity,
        TCoreEntity,
        TModel,
        IEnumerable<TApi>
    > GuildRelatedEntity<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        [Ignore] TRestEntity,
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface] [RequireResolve, Shrink]
        TCoreEntity,
        TModel,
        [Not(nameof(TModel)), Shrink] TApi,
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
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, TContext, DiscordRestClient>
        where TCoreEntity : class, IEntity<TId, TModel>, IFetchableOfMany<TId, TApi>
        where TModel : class, TApi
        where TApi : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCoreEntity, TModel,
            IEnumerable<TApi>>(
            client,
            id => TActor.Factory(client, guild, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            model => TRestEntity.Construct(
                client.Rest,
                context(guild),
                model
            ),
            path,
            TCoreEntity.FetchManyRoute(path),
            api => api.OfType<TModel>()
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
        [Not(nameof(TModel)), Shrink] TApi,
        TContext
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path,
        Func<GuildIdentity, TContext> context,
        IApiOutRoute<TApi> route,
        Func<TApi, IEnumerable<TModel>> transform
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
        IContextConstructable<TEntity, TModel, IGatewayConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, TContext, DiscordRestClient>
        where TCoreEntity : class, IEntity<TId, TModel>
        where TModel : class, IEntityModel<TId>
        where TApi : class
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
            route,
            transform
        );
    }

    public static GatewayGuildInvites GuildInvites(
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path
    ) => new(
        client,
        id => new GatewayGuildInviteActor(client, guild, GuildInviteIdentity.Of(id)),
        model => RestGuildInvite.Construct(
            client.Rest,
            new RestGuildInvite.Context(
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id)
            ),
            model
        ),
        path,
        IGuildInvite.FetchManyRoute(path)
    );

    public static GatewayGuildChannelInvites ChannelInvites(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildChannelIdentity channel,
        CachePathable path)
    {
        return new GatewayGuildChannelInvites(
            client,
            id => client.Guilds[guild].Channels[channel].Invites[id],
            model => RestGuildChannelInvite.Construct(
                client.Rest,
                new RestGuildChannelInvite.Context(
                    RestGuildIdentity.Of(guild.Id),
                    RestGuildChannelIdentity.Of(channel.Id)
                ),
                model
            ),
            path,
            IGuildChannelInvite.FetchManyRoute(path)
        );
    }

    public static BansPager PageBans(DiscordGatewayClient client, GuildIdentity guild, CachePathable path)
        => GatewayPagedIndexableActor.Create<RestBan, PageGuildBansParams>(
            Template.Of<GatewayBanActor>(),
            client,
            id => new GatewayBanActor(client, guild, BanIdentity.Of(id)),
            path,
            api => api,
            (model, _) => RestBan.Construct(
                client.Rest,
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id),
                model
            )
        );

    public static GuildsPager PageGuilds(DiscordGatewayClient client)
        => GatewayPagedIndexableActor.CreatePartial<RestPartialGuild, PageUserGuildsParams>(
            Template.Of<GatewayGuildActor>(),
            client,
            CachePathable.Empty,
            id => new GatewayGuildActor(client, GuildIdentity.Of(id)),
            api => api,
            (model, _) => RestPartialGuild.Construct(client.Rest, model)
        );

    public static MembersPager PageMembers(DiscordGatewayClient client, GuildIdentity guild, CachePathable path)
        => GatewayPagedIndexableActor.Create<RestMember, PageGuildMembersParams>(
            Template.Of<GatewayMemberActor>(),
            client,
            id => new GatewayMemberActor(client, guild, MemberIdentity.Of(id)),
            path,
            api => api,
            (model, _) => RestMember.Construct(
                client.Rest,
                IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>.Of(guild.Id),
                model
            )
        );
}
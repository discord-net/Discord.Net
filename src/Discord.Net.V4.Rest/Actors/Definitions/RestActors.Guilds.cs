using Discord.Models;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestPartialPagedIndexableLink<
        RestGuildActor,
        ulong,
        RestGuild,
        RestPartialGuild,
        IPartialGuildModel,
        IEnumerable<IPartialGuildModel>,
        PageUserGuildsParams
    > PagedGuilds(
        DiscordRestClient client)
    {
        return new RestPartialPagedIndexableLink<RestGuildActor, ulong, RestGuild, RestPartialGuild, IPartialGuildModel
            , IEnumerable<IPartialGuildModel>, PageUserGuildsParams>(
            client,
            id => new RestGuildActor(client, GuildIdentity.Of(id)),
            IPathable.Empty,
            x => x,
            (model, _) => RestPartialGuild.Construct(client, model)
        );
    }

    public static RestEnumerableIndexableLink<
        TTrait,
        TId,
        TRestEntity,
        TCoreEntity,
        IEnumerable<TModel>
    > GuildRelatedTrait<
        [TransitiveFill] TTrait,
        TRootActor,
        TRestEntity,
        [Not(nameof(TRestEntity)), Interface] TCoreEntity,
        TModel,
        TId
    >(
        Template<TTrait> template,
        Template<TRootActor> rootTemplate,
        DiscordRestClient client,
        RestGuildActor guild,
        IApiOutRoute<IEnumerable<TModel>> route,
        Func<TModel, bool> filter
    )
        where TTrait :
        class,
        IRestTrait<TId, TRestEntity>,
        IActorTrait<TId, TCoreEntity>,
        IFactory<
            TTrait,
            DiscordRestClient,
            TRootActor,
            IIdentifiable<TId, TCoreEntity, TTrait, TModel>
        >
        where TRootActor :
        class,
        IActor<TId, TRestEntity>,
        IFactory<
            TRootActor,
            DiscordRestClient,
            IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            IIdentifiable<TId, TRestEntity, TRootActor, TModel>
        >
        where TRestEntity :
        RestEntity<TId>,
        IEntity<TId, TModel>,
        IContextConstructable<
            TRestEntity,
            TModel,
            IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient
        >
        where TCoreEntity :
        class,
        IEntity<TId, TModel>,
        IFetchableOfMany<TId, TModel>
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        return RestEnumerableIndexableActor
            .CreateTrait<TTrait, TId, TRestEntity, TCoreEntity, IEnumerable<TModel>, TModel>(
                client,
                id => TTrait.Factory(
                    client,
                    TRootActor.Factory(
                        client,
                        guild.Identity,
                        IIdentifiable<TId, TRestEntity, TRootActor, TModel>.Of(id)
                    ),
                    IIdentifiable<TId, TCoreEntity, TTrait, TModel>.Of(id)
                ),
                models => models.Select(model =>
                    (TRestEntity)TRestEntity.Construct(
                        client,
                        guild.Identity,
                        model
                    )
                ),
                route,
                models => models.Where(filter)
            );
    }

    public static RestEnumerableIndexableLink<TActor, TId, TRestEntity, TCoreEntity,
            IEnumerable<TRouteModel>>
        GuildRelatedEntityWithTransform<
            [TransitiveFill] TActor,
            TRestEntity,
            [Not(nameof(TRestEntity)), Interface, Shrink]
            TCoreEntity,
            TModel,
            TRouteModel,
            TApiModel,
            TId
        >(
            Template<TActor> template,
            DiscordRestClient client,
            RestGuildActor guild,
            IApiOutRoute<TApiModel> route,
            Func<TApiModel, IEnumerable<TRouteModel>> transform
        )
        where TActor :
        class,
        IRestActor<TId, TRestEntity, IIdentifiable<TId, TRestEntity, TActor, TModel>>,
        IFactory<TActor, DiscordRestClient, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            IIdentifiable<TId, TRestEntity, TActor, TModel>>
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, IIdentifiable<ulong, RestGuild, RestGuildActor, IGuildModel>,
            DiscordRestClient>
        where TCoreEntity :
        class,
        IEntity<TId, TModel>,
        IFetchableOfMany<TId, TRouteModel>
        where TModel : class, IEntityModel<TId>, TRouteModel
        where TRouteModel : class, IEntityModel<TId>
        where TApiModel : class
        where TId : IEquatable<TId>
    {
        return RestEnumerableIndexableActor.Create<TActor, TId, TRestEntity, TCoreEntity, TApiModel, TRouteModel>(
            client,
            id => TActor.Factory(client, guild.Identity,
                IIdentifiable<TId, TRestEntity, TActor, TModel>.Of(id)),
            models => models.OfType<TModel>()
                .Select(model => (TRestEntity)TRestEntity.Construct(client, guild.Identity, model)),
            route,
            transform
        );
    }

    public static RestEnumerableIndexableLink<TActor, TId, TRestEntity, TCoreEntity,
            IEnumerable<TRouteModel>>
        GuildRelatedEntity<
            [TransitiveFill] TActor,
            TRestEntity,
            [Not(nameof(TRestEntity)), Interface, Shrink]
            TCoreEntity,
            TModel,
            TRouteModel,
            TId
        >(
            Template<TActor> template,
            DiscordRestClient client,
            RestGuildActor guild
        )
        where TActor :
        class,
        IRestActor<TId, TRestEntity, IIdentifiable<TId, TRestEntity, TActor, TModel>>,
        IFactory<TActor, DiscordRestClient, GuildIdentity, IIdentifiable<TId, TRestEntity, TActor, TModel>>
        where TRestEntity :
        RestEntity<TId>,
        TCoreEntity,
        IContextConstructable<TRestEntity, TModel, GuildIdentity, DiscordRestClient>
        where TCoreEntity :
        class,
        IEntity<TId, TModel>,
        IFetchableOfMany<TId, TRouteModel>
        where TModel : class, TRouteModel
        where TRouteModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        return RestEnumerableIndexableActor.Create<TActor, TId, TRestEntity, TCoreEntity, TRouteModel>(
            client,
            id => TActor.Factory(client, guild.Identity,
                IIdentifiable<TId, TRestEntity, TActor, TModel>.Of(id)),
            models => models.OfType<TModel>()
                .Select(model => (TRestEntity)TRestEntity.Construct(client, guild.Identity, model)),
            TCoreEntity.FetchManyRoute(guild)
        );
    }

    public static
        RestPagedIndexableLink<RestMemberActor, ulong, RestMember, IMemberModel, IEnumerable<IMemberModel>,
            PageGuildMembersParams> Members(DiscordRestClient client, RestGuildActor guild)
    {
        return new(
            client,
            memberId => new(client, guild.Identity, MemberIdentity.Of(memberId)),
            guild,
            x => x,
            (model, _) => RestMember.Construct(client, guild.Identity, model)
        );
    }

    public static RestPagedIndexableLink<RestBanActor, ulong, RestBan, IBanModel, IEnumerable<IBanModel>,
            PageGuildBansParams>
        Bans(
            DiscordRestClient client,
            RestGuildActor guild)
    {
        return new RestPagedIndexableLink<RestBanActor, ulong, RestBan, IBanModel, IEnumerable<IBanModel>,
            PageGuildBansParams>(
            client,
            id => new RestBanActor(client, guild.Identity, BanIdentity.Of(id)),
            guild,
            x => x,
            (model, _) => RestBan.Construct(client, guild.Identity, model)
        );
    }
}

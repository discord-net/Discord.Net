using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Guilds.Integrations;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestEnumerableIndexableActor<TActor, TId, TRestEntity, TCoreEntity,
            IEnumerable<TRouteModel>>
        GuildRelatedEntityWithTransform<
            [TransitiveFill] TActor,
            TRestEntity,
            [Not(nameof(TRestEntity)), Interface] TCoreEntity,
            TModel,
            TRouteModel,
            TApiModel,
            TIdentity,
            TGuildIdentity,
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
            IRestActor<TId, TRestEntity, TIdentity>,
            IFactory<TActor, DiscordRestClient, TGuildIdentity, IIdentifiable<TId, TRestEntity, TActor, TModel>>
        where TRestEntity :
            RestEntity<TId>,
            TCoreEntity,
            IContextConstructable<TRestEntity, TModel, TGuildIdentity, DiscordRestClient>
        where TCoreEntity :
            class,
            IEntity<TId>,
            IEntityOf<TModel>,
            IFetchableOfMany<TId, TRouteModel>
        where TModel : class, IEntityModel<TId>, TRouteModel
        where TRouteModel : class, IEntityModel<TId>
        where TApiModel : class
        where TId : IEquatable<TId>
        where TGuildIdentity : class?, GuildIdentity?
    {
        return RestEnumerableIndexableActor.Create<TActor, TId, TRestEntity, TCoreEntity, TApiModel, TRouteModel>(
            client,
            id => TActor.Factory(client, (TGuildIdentity)guild.Identity,
                IIdentifiable<TId, TRestEntity, TActor, TModel>.Of(id)),
            models => models.OfType<TModel>().Select(model => (TRestEntity)TRestEntity.Construct(client, (TGuildIdentity)guild.Identity, model)),
            route,
            transform
        );
    }

    public static RestEnumerableIndexableActor<TActor, TId, TRestEntity, TCoreEntity,
            IEnumerable<TRouteModel>>
        GuildRelatedEntity<
            [TransitiveFill] TActor,
            TRestEntity,
            [Not(nameof(TRestEntity)), Interface] TCoreEntity,
            TModel,
            TRouteModel,
            TIdentity,
            TId,
            TGuildIdentity
        >(
            Template<TActor> template,
            DiscordRestClient client,
            RestGuildActor guild
        )
        where TActor :
            class,
            IRestActor<TId, TRestEntity, TIdentity>,
            IFactory<TActor, DiscordRestClient, TGuildIdentity, IIdentifiable<TId, TRestEntity, TActor, TModel>>
        where TRestEntity :
            RestEntity<TId>,
            TCoreEntity,
            IContextConstructable<TRestEntity, TModel, TGuildIdentity, DiscordRestClient>
        where TCoreEntity :
            class,
            IEntity<TId>,
            IEntityOf<TModel>,
            IFetchableOfMany<TId, TRouteModel>
        where TModel : class, TRouteModel
        where TRouteModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
        where TGuildIdentity : class?, GuildIdentity?
    {
        return RestEnumerableIndexableActor.Create<TActor, TId, TRestEntity, TCoreEntity, TRouteModel>(
            client,
            id => TActor.Factory(client, (TGuildIdentity)guild.Identity,
                IIdentifiable<TId, TRestEntity, TActor, TModel>.Of(id)),
            models => models.OfType<TModel>().Select(model => (TRestEntity)TRestEntity.Construct(client, (TGuildIdentity)guild.Identity, model)),
            TCoreEntity.FetchManyRoute(guild)
        );
    }

    public static
        RestPagedIndexableActor<RestGuildMemberActor, ulong, RestGuildMember, IEnumerable<IMemberModel>,
            PageGuildMembersParams> Members(DiscordRestClient client, GuildIdentity guild)
    {
        return new(
            client,
            memberId => new(client, guild, MemberIdentity.Of(memberId)),
            args => Routes.ListGuildMembers(guild.Id, args.PageSize, args.After?.Id),
            (_, models) => models
                .Select(x =>
                    RestGuildMember.Construct(client, guild, x)
                ),
            (_, models, args) =>
            {
                var nextId = models.MaxBy(x => x.Id)?.Id;

                if (!nextId.HasValue)
                    return null;

                return Routes.ListGuildMembers(
                    guild.Id,
                    args.PageSize,
                    nextId
                );
            }
        );
    }

    public static RestPagedIndexableActor<RestBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams>
        Bans(
            DiscordRestClient client,
            GuildIdentity guild)
    {
        return new RestPagedIndexableActor<RestBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams>(
            client,
            banId => new(client, guild, BanIdentity.Of(banId)),
            args => Routes.GetGuildBans(guild.Id, args.PageSize, args.Before?.Id, args.After?.Id),
            (_, models) => models.Select(x => RestBan.Construct(client, guild, x)),
            (_, models, args) =>
            {
                ulong? nextId;

                if (args.After.HasValue)
                {
                    nextId = models?.MaxBy(x => x.Id)?.Id;

                    if (!nextId.HasValue)
                        return null;

                    return Routes.GetGuildBans(
                        guild.Id,
                        args.PageSize,
                        after: nextId
                    );
                }

                nextId = models.MinBy(x => x.Id)?.Id;

                if (!nextId.HasValue)
                    return null;

                return Routes.GetGuildBans(
                    guild.Id,
                    args.PageSize,
                    before: nextId
                );
            }
        );
    }
}

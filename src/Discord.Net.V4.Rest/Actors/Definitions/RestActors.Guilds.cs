using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Guilds.Integrations;

namespace Discord.Rest;

internal static partial class RestActors
{
    // public static RestEnumerableIndexableActor<TChannelActor, ulong, TRestChannel, TCoreChannel,
    //         IEnumerable<IChannelModel>>
    //     GuildChannels<
    //         [TransitiveFill] TChannelActor,
    //         TRestChannel,
    //         [Not(nameof(TRestChannel)), Interface] TCoreChannel,
    //         TChannelModel,
    //         TIdentity
    //     >(
    //         DiscordRestClient client,
    //         GuildIdentity guild,
    //         IApiOutRoute<IEnumerable<TChannelModel>>? route = null)
    //     where TChannelActor :
    //     RestGuildChannelActor,
    //     IRestActor<ulong, TRestChannel, TIdentity>,
    //     IFactory<TChannelActor, DiscordRestClient, GuildIdentity,
    //         IIdentifiableEntityOrModel<ulong, TRestChannel, TChannelModel>>
    //     where TRestChannel :
    //     RestGuildChannel,
    //     TCoreChannel,
    //     IEntityOf<TChannelModel>,
    //     IContextConstructable<TRestChannel, TChannelModel, GuildIdentity, DiscordRestClient>
    //     where TCoreChannel : class, IGuildChannel
    //     where TChannelModel : class, IGuildChannelModel
    // {
    //     return RestEnumerableIndexableActor.Create<TChannelActor, ulong, TRestChannel, TCoreChannel, IChannelModel>(
    //         client,
    //         id => TChannelActor.Factory(client, guild,
    //             IIdentifiableEntityOrModel<ulong, TRestChannel, TChannelModel>.Of(id)),
    //         models => models.OfType<TChannelModel>().Select(model => TRestChannel.Construct(client, guild, model)),
    //         route ?? (IApiOutRoute<IEnumerable<IChannelModel>>)Routes.GetGuildChannels(guild.Id)
    //     );
    // }

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
            TId
        >(
            DiscordRestClient client,
            RestGuildActor guild,
            IApiOutRoute<TApiModel> route,
            Func<TApiModel, IEnumerable<TRouteModel>> transform
        )
        where TActor :
            IRestActor<TId, TRestEntity, TIdentity>,
            IFactory<TActor, DiscordRestClient, GuildIdentity, IIdentifiableEntityOrModel<TId, TRestEntity, TModel>>
        where TRestEntity :
            RestEntity<TId>,
            TCoreEntity,
            IContextConstructable<TRestEntity, TModel, GuildIdentity, DiscordRestClient>
        where TCoreEntity :
            class,
            IEntity<TId>,
            IEntityOf<TModel>,
            IFetchableOfMany<TId, TRouteModel>
        where TModel : class, IEntityModel<TId>, TRouteModel
        where TRouteModel : class, IEntityModel<TId>
        where TApiModel : class
        where TId : IEquatable<TId>
    {
        return RestEnumerableIndexableActor.Create<TActor, TId, TRestEntity, TCoreEntity, TApiModel, TRouteModel>(
            client,
            id => TActor.Factory(client, guild.Identity,
                IIdentifiableEntityOrModel<TId, TRestEntity, TModel>.Of(id)),
            models => models.OfType<TModel>().Select(model => (TRestEntity)TRestEntity.Construct(client, guild.Identity, model)),
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
            TId
        >(
            DiscordRestClient client,
            RestGuildActor guild
        )
        where TActor :
            IRestActor<TId, TRestEntity, TIdentity>,
            IFactory<TActor, DiscordRestClient, GuildIdentity, IIdentifiableEntityOrModel<TId, TRestEntity, TModel>>
        where TRestEntity :
            RestEntity<TId>,
            TCoreEntity,
            IContextConstructable<TRestEntity, TModel, GuildIdentity, DiscordRestClient>
        where TCoreEntity :
            class,
            IEntity<TId>,
            IEntityOf<TModel>,
            IFetchableOfMany<TId, TRouteModel>
        where TModel : class, TRouteModel
        where TRouteModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        return RestEnumerableIndexableActor.Create<TActor, TId, TRestEntity, TCoreEntity, TRouteModel>(
            client,
            id => TActor.Factory(client, guild.Identity,
                IIdentifiableEntityOrModel<TId, TRestEntity, TModel>.Of(id)),
            models => models.OfType<TModel>().Select(model => TRestEntity.Construct(client, guild.Identity, model)),
            TCoreEntity.FetchManyRoute(guild)
        );
    }

    public static RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IIntegration,
            IEnumerable<IIntegrationModel>>
        Integrations(
            DiscordRestClient client,
            GuildIdentity guild)
    {
        return new RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IIntegration,
            IEnumerable<IIntegrationModel>>(
            client,
            integrationId => new RestIntegrationActor(client, guild, IntegrationIdentity.Of(integrationId)),
            model => model.Select(x => RestIntegration.Construct(client, guild, x)),
            Routes.GetGuildIntegrations(guild.Id)
        );
    }

    public static RestIndexableActor<RestThreadChannelActor, ulong, RestThreadChannel> Threads(
        DiscordRestClient client,
        GuildIdentity guild
    ) => new(threadId => new(client, guild, ThreadIdentity.Of(threadId)));

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

        // return new RestPagedIndexableActor<RestBanActor, ulong, RestBan, IBanModel>(
        //     client,
        //     banId => new RestBanActor(client, guildId, banId),
        //     Routes.GetGuildBans()
        // );
    }
}

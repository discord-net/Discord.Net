using Discord.Gateway.State;
using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

using GuildsPager = GatewayPagedIndexableActor<GatewayGuildActor, ulong, GatewayGuild, RestPartialGuild,
    PageUserGuildsParams, IEnumerable<IPartialGuildModel>>;

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
        TRestEntity,
        [Not(nameof(TRestEntity)), Not(nameof(TEntity)), Interface]TCoreEntity,
        TModel
    >(
        Template<TActor> template,
        DiscordGatewayClient client,
        GuildIdentity guild
    )
        where TActor :
        class,
        IGatewayCachedActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>, TModel>,
        IFactory<TActor, DiscordGatewayClient, GuildIdentity, IIdentifiable<TId, TEntity, TActor, TModel>>,
        IStoreProvider<TId, TModel>
        where TId : IEquatable<TId>
        where TEntity :
        GatewayEntity<TId>,
        ICacheableEntity<TEntity, TId, TModel>,
        IStoreProvider<TId, TModel>,
        IBrokerProvider<TId, TEntity, TModel>,
        IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>,
        TCoreEntity
        where TRestEntity : RestEntity<TId>, TCoreEntity
        where TCoreEntity : class, IEntity<TId>
        where TModel : class, IEntityModel<TId>
    {
        return new GatewayEnumerableIndexableActor<TActor, TId, TEntity, TRestEntity, TCoreEntity, TModel>(
            client,
            id => TActor.Factory(client, guild, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),

        );
    }

    public static GuildsPager PageGuilds(DiscordGatewayClient client)
    {
        return new GuildsPager(
            client,
            id => new GatewayGuildActor(client, GuildIdentity.Of(id)),
            pageParams => Routes.GetCurrentUserGuilds(pageParams.Before?.Id, pageParams.After?.Id, pageParams.PageSize, true),
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

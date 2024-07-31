using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

using GuildsPager = GatewayPagedIndexableActor<GatewayGuildActor, ulong, GatewayGuild, RestPartialGuild,
    PageUserGuildsParams, IEnumerable<IPartialGuildModel>>;

internal static partial class GatewayActors
{
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

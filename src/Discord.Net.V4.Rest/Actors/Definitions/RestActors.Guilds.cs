using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Guilds.Integrations;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static
        RestEnumerableIndexableActor<RestLoadableStageChannelActor, ulong, RestStageChannel, IEnumerable<IChannelModel>>
        StageChannels(
            DiscordRestClient client,
            IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild
        )
    {
        return GuildChannel<RestLoadableStageChannelActor, RestStageChannel, IGuildStageChannelModel>(
            client,
            guild,
            id => new RestLoadableStageChannelActor(client, guild, id),
            model => RestStageChannel.Construct(client, model, guildId)
        );
    }

    public static RestEnumerableIndexableActor<TChannelActor, ulong, TChannel, IEnumerable<IChannelModel>>
        GuildChannel<TChannelActor, TChannel, TChannelModel>(
            DiscordRestClient client,
            IdentifiableEntityOrModel<ulong, RestGuild, IGuildModel> guild,
            Func<ulong, TChannelActor> actorFactory,
            Func<TChannelModel, TChannel> factory
        )
        where TChannel : RestGuildChannel
        where TChannelActor : RestGuildChannelActor, IActor<ulong, TChannel>
        where TChannelModel : class, IGuildChannelModel
    {
        return new RestEnumerableIndexableActor<TChannelActor, ulong, TChannel, IEnumerable<IChannelModel>>(
            client,
            actorFactory,
            models => models.OfType<TChannelModel>().Select(factory),
            Routes.GetGuildChannels(guild.Id)
        );
    }

    public static RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IEnumerable<IIntegrationModel>>
        Integrations(DiscordRestClient client, ulong guildId, RestGuild? guild)
    {
        return new RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IEnumerable<IIntegrationModel>>(
            client,
            integrationId => new RestIntegrationActor(client, guildId, integrationId, guild),
            model => model.Select(x => RestIntegration.Construct(client, x, new(
                guildId,
                guild
            ))),
            Routes.GetGuildIntegrations(guildId)
        );
    }

    public static RestIndexableActor<RestLoadableThreadChannelChannelActor, ulong, RestThreadChannel> Threads(DiscordRestClient client, ulong guildId)
        => new(threadId => new(client, guildId, threadId));

    public static
        RestPagedIndexableActor<RestLoadableGuildMemberActor, ulong, RestGuildMember, IEnumerable<IMemberModel>,
            PageGuildMembersParams> Members(DiscordRestClient client, ulong guildId)
    {
        return new(
            client,
            memberId => new(client, guildId, memberId),
            args => Routes.ListGuildMembers(guildId, args.PageSize, args.After?.Id),
            (_, models) => models
                .Select(x =>
                    RestGuildMember.Construct(client, x, new(guildId, x.Id!.Value))
                ),
            (_, models, args) =>
            {
                var nextId = models.MaxBy(x => x.Id)?.Id;

                if (!nextId.HasValue)
                    return null;

                return Routes.ListGuildMembers(
                    guildId,
                    args.PageSize,
                    nextId
                );
            }
        );
    }

    public static RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams> Bans(DiscordRestClient client, ulong guildId)
    {
        return new RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams>(
            client,
            banId => new(client, guildId, banId),
            args => Routes.GetGuildBans(guildId, args.PageSize, args.Before?.Id, args.After?.Id),
            (_, models) => models.Select(x=> RestBan.Construct(client, x, guildId)),
            (_, models, args) =>
            {
                ulong? nextId;

                if (args.After.HasValue)
                {
                    nextId = models?.MaxBy(x => x.Id)?.Id;

                    if (!nextId.HasValue)
                        return null;

                    return Routes.GetGuildBans(
                        guildId,
                        args.PageSize,
                        afterId: nextId
                    );
                }

                nextId = models.MinBy(x => x.Id)?.Id;

                if (!nextId.HasValue)
                    return null;

                return Routes.GetGuildBans(
                    guildId,
                    args.PageSize,
                    beforeId: nextId
                );
            }
        );

        // return new RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IBanModel>(
        //     client,
        //     banId => new RestLoadableBanActor(client, guildId, banId),
        //     Routes.GetGuildBans()
        // );
    }
}

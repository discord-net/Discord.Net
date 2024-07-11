using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;
using Discord.Rest.Guilds.Integrations;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static
        RestEnumerableIndexableActor<RestLoadableStageChannelActor, ulong, RestStageChannel, IStageChannel, IEnumerable<IChannelModel>>
        StageChannels(
            DiscordRestClient client,
            GuildIdentity guild
        )
    {
        return GuildChannel<RestLoadableStageChannelActor, RestStageChannel, IStageChannel, IGuildStageChannelModel>(
            client,
            guild,
            id => new RestLoadableStageChannelActor(client, guild, id),
            model => RestStageChannel.Construct(client, model, guild)
        );
    }

    public static RestEnumerableIndexableActor<TChannelActor, ulong, TRestChannel, TCoreChannel, IEnumerable<IChannelModel>>
        GuildChannel<TChannelActor, TRestChannel, TCoreChannel, TChannelModel>(
            DiscordRestClient client,
            GuildIdentity guild,
            Func<ulong, TChannelActor> actorFactory,
            Func<TChannelModel, TRestChannel> factory
        )
        where TRestChannel : RestGuildChannel, TCoreChannel
        where TCoreChannel : class, IGuildChannel
        where TChannelActor : RestGuildChannelActor, IActor<ulong, TRestChannel>
        where TChannelModel : class, IGuildChannelModel
    {
        return new RestEnumerableIndexableActor<TChannelActor, ulong, TRestChannel, TCoreChannel, IEnumerable<IChannelModel>>(
            client,
            actorFactory,
            models => models.OfType<TChannelModel>().Select(factory),
            Routes.GetGuildChannels(guild.Id)
        );
    }

    public static RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IIntegration, IEnumerable<IIntegrationModel>>
        Integrations(
            DiscordRestClient client,
            GuildIdentity guild)
    {
        return new RestEnumerableIndexableActor<RestIntegrationActor, ulong, RestIntegration, IIntegration, IEnumerable<IIntegrationModel>>(
            client,
            integrationId => new RestIntegrationActor(client, guild, IntegrationIdentity.Of(integrationId)),
            model => model.Select(x => RestIntegration.Construct(client, x, guild)),
            Routes.GetGuildIntegrations(guild.Id)
        );
    }

    public static RestIndexableActor<RestLoadableThreadChannelActor, ulong, RestThreadChannel> Threads(
        DiscordRestClient client,
        GuildIdentity guild
    ) => new(threadId => new(client, guild, ThreadIdentity.Of(threadId)));

    public static
        RestPagedIndexableActor<RestLoadableGuildMemberActor, ulong, RestGuildMember, IEnumerable<IMemberModel>,
            PageGuildMembersParams> Members(DiscordRestClient client, GuildIdentity guild)
    {
        return new(
            client,
            memberId => new(client, guild, MemberIdentity.Of(memberId)),
            args => Routes.ListGuildMembers(guild.Id, args.PageSize, args.After?.Id),
            (_, models) => models
                .Select(x =>
                    RestGuildMember.Construct(client, x, guild)
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

    public static RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams> Bans(
        DiscordRestClient client,
        GuildIdentity guild)
    {
        return new RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IEnumerable<IBanModel>, PageGuildBansParams>(
            client,
            banId => new(client, guild, BanIdentity.Of(banId)),
            args => Routes.GetGuildBans(guild.Id, args.PageSize, args.Before?.Id, args.After?.Id),
            (_, models) => models.Select(x=> RestBan.Construct(client, x, guild)),
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
                        afterId: nextId
                    );
                }

                nextId = models.MinBy(x => x.Id)?.Id;

                if (!nextId.HasValue)
                    return null;

                return Routes.GetGuildBans(
                    guild.Id,
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

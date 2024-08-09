using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestPagedIndexableActor<
        RestThreadChannelActor,
        ulong,
        RestThreadChannel,
        IThreadChannelModel,
        ChannelThreads,
        PagePublicArchivedThreadsParams
    > PublicArchivedThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel
    ) => ListThreads<PagePublicArchivedThreadsParams>(client, guild, channel);

    public static RestPagedIndexableActor<
        RestThreadChannelActor,
        ulong,
        RestThreadChannel,
        IThreadChannelModel,
        ChannelThreads,
        PagePrivateArchivedThreadsParams
    > PrivateArchivedThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel
    ) => ListThreads<PagePrivateArchivedThreadsParams>(client, guild, channel);

    public static RestPagedIndexableActor<
        RestThreadChannelActor,
        ulong,
        RestThreadChannel,
        IThreadChannelModel,
        ChannelThreads,
        PageJoinedPrivateArchivedThreadsParams
    > JoinedPrivateArchivedThreads(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel
    ) => ListThreads<PageJoinedPrivateArchivedThreadsParams>(client, guild, channel);

    private static RestPagedIndexableActor<
        RestThreadChannelActor,
        ulong,
        RestThreadChannel,
        IThreadChannelModel,
        ChannelThreads,
        TParams
    > ListThreads<TParams>(
        DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel)
        where TParams : class, IPagingParams<TParams, ChannelThreads>
    {
        return new RestPagedIndexableActor<RestThreadChannelActor, ulong, RestThreadChannel, IThreadChannelModel,
            ChannelThreads, TParams>(
            client,
            id => new RestThreadChannelActor(client, guild, ThreadIdentity.Of(id)),
            IPathable.Empty,
            api => api.Threads,
            (model, api) =>
            {
                return RestThreadChannel.Construct(
                    client,
                    new(
                        guild,
                        ThreadMemberIdentity.OfNullable(
                            api.Members.FirstOrDefault(x => x.ThreadId == model.Id),
                            model => RestThreadMember.Construct(
                                client,
                                new(guild, ThreadIdentity.Of(model.ThreadId!.Value)),
                                model
                            )
                        ),
                        channel
                    ),
                    model
                );
            }
        );
    }
}

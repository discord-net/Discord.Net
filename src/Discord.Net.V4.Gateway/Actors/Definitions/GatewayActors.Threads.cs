using Discord.Gateway.State;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Microsoft.Extensions.Options;

namespace Discord.Gateway;

using RestThreadIdentity = IIdentifiable<ulong, RestThreadChannel, RestThreadChannelActor, IThreadChannelModel>;
using RestThreadMemberIdentity = IIdentifiable<ulong, RestThreadMember, RestThreadMemberActor, IThreadMemberModel>;
using RestMemberIdentity = IIdentifiable<ulong, RestMember, RestMemberActor, IMemberModel>;

internal static partial class GatewayActors
{
    public static PublicArchivedThreadsPager PublicArchivedThreads(
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path)
    {
        var restGuild = RestGuildIdentity.Of(guild.Id);

        return GatewayPagedIndexableActor.Create<RestThreadChannel, PagePublicArchivedThreadsParams>(
            Template.Of<GatewayThreadChannelActor>(),
            client,
            id => new GatewayThreadChannelActor(
                client,
                guild,
                ThreadChannelIdentity.Of(id)
            ),
            path,
            api => api.Threads,
            (model, api) => CreateRestThreadChannel(client.Rest, restGuild, model, api),
            (channelThreads, options, token) => ProcessChannelThreadsDataAsync(
                client,
                guild,
                channelThreads,
                options,
                token
            )
        );
    }

    public static PrivateArchivedThreadsPager PrivateArchivedThreads(
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path)
    {
        var restGuild = RestGuildIdentity.Of(guild.Id);

        return GatewayPagedIndexableActor.Create<RestThreadChannel, PagePrivateArchivedThreadsParams>(
            Template.Of<GatewayThreadChannelActor>(),
            client,
            id => new GatewayThreadChannelActor(
                client,
                guild,
                ThreadChannelIdentity.Of(id)
            ),
            path,
            api => api.Threads,
            (model, api) => CreateRestThreadChannel(client.Rest, restGuild, model, api),
            (channelThreads, options, token) => ProcessChannelThreadsDataAsync(
                client,
                guild,
                channelThreads,
                options,
                token
            )
        );
    }

    public static JoinedPrivateArchivedThreadsPager JoinedPrivateArchivedThreads(
        DiscordGatewayClient client,
        GuildIdentity guild,
        CachePathable path)
    {
        var restGuild = RestGuildIdentity.Of(guild.Id);

        return GatewayPagedIndexableActor.Create<RestThreadChannel, PageJoinedPrivateArchivedThreadsParams>(
            Template.Of<GatewayThreadChannelActor>(),
            client,
            id => new GatewayThreadChannelActor(
                client,
                guild,
                ThreadChannelIdentity.Of(id)
            ),
            path,
            api => api.Threads,
            (model, api) => CreateRestThreadChannel(client.Rest, restGuild, model, api),
            (channelThreads, options, token) => ProcessChannelThreadsDataAsync(
                client,
                guild,
                channelThreads,
                options,
                token
            )
        );
    }

    private static RestThreadChannel CreateRestThreadChannel(
        DiscordRestClient client,
        RestGuildIdentity guild,
        IThreadChannelModel model,
        ChannelThreads channelThreads)
    {
        // this is not my proudest bit of doohickery :/
        // we want to consume all the data from the api, this ensures that.
        var currentThreadMemberIdentity = RestThreadMemberIdentity.FromReferenced(
            channelThreads,
            model.Id,
            model => RestThreadMember.Construct(
                client,
                new RestThreadMember.Context(
                    guild,
                    RestThreadIdentity.Of(model.Id),
                    RestMemberIdentity.FromReferenced(
                        model,
                        client.CurrentUser.Id,
                        model => RestMember.Construct(client, guild, model)
                    )
                ),
                model
            )
        );

        return RestThreadChannel.Construct(
            client,
            new RestThreadChannel.Context(
                RestGuildIdentity.Of(guild.Id),
                currentThreadMemberIdentity
            ),
            model
        );
    }

    private static async ValueTask ProcessChannelThreadsDataAsync(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ChannelThreads channelThreads,
        RequestOptions options,
        CancellationToken token)
    {
        if (options is GatewayRequestOptions {UpdateCache: true})
        {
            var broker = GatewayThreadMemberActor.GetBroker(client);

            await Parallel.ForEachAsync(
                channelThreads.Members.Where(x => x.ThreadId.IsSpecified),
                token,
                async (model, token) =>
                {
                    var storeInfo = await GatewayThreadMemberActor.GetStoreInfoAsync(
                        client,
                        new CachePathable {guild, ThreadChannelIdentity.Of(model.ThreadId.Value)},
                        token
                    );

                    await broker.UpdateAsync(model, storeInfo, token);
                });
        }
    }
}

using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Models;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildAnnouncementChannel)),
    Creatable<CreateGuildAnnouncementChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor),
        RouteGenerics = [typeof(GuildAnnouncementChannel)]
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IAnnouncementChannelActor :
    IMessageChannelTrait,
    IIntegrationChannelTrait.WithIncoming.WithChannelFollower,
    IThreadableChannelTrait<IAnnouncementThreadChannelActor.Indexable.WithAnnouncementArchived.BackLink<IAnnouncementChannelActor>>,
    IActor<ulong, IAnnouncementChannel>
{
    async Task<FollowedChannel> FollowAnnouncementChannelAsync(EntityOrId<ulong, ITextChannel> channel,
        RequestOptions? options = null, CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.FollowAnnouncementChannel(
                Id,
                new FollowAnnouncementChannelParams {WebhookChannelId = channel.Id}),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return FollowedChannel.Construct(Client, model);
    }
}
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildAnnouncementChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface INewsChannelActor :
    ITextChannelActor,
    IActor<ulong, INewsChannel>
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

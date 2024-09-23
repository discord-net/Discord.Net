using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Models.Json;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook), typeof(IChannelFollowerWebhookModel))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IChannelFollowerWebhookActor :
    IGuildChannelWebhookActor,
    IActor<ulong, IChannelFollowerWebhook>
{
    [BackLink<INewsChannelActor>]
    private static async Task<FollowedChannel> FollowAsync(
        INewsChannelActor newsChannelActor,
        EntityOrId<ulong, IChannelActor> channel,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return FollowedChannel
            .Construct(
                newsChannelActor.Client,
                await newsChannelActor.Client.RestApiClient.ExecuteRequiredAsync(
                    Routes.FollowAnnouncementChannel(
                        newsChannelActor.Id,
                        new FollowAnnouncementChannelParams()
                        {
                            WebhookChannelId = channel.Id
                        }
                    ),
                    options ?? newsChannelActor.Client.DefaultRequestOptions,
                    token
                )
            );
    }
}
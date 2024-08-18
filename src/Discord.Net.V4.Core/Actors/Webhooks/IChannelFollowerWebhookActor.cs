using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[ActorCreatableAttribute<BacklinkFollowAnnouncementChannelProperties>(
    nameof(Routes.FollowAnnouncementChannel),
    nameof(Models.Json.FollowedChannel.WebhookId)
)]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IChannelFollowerWebhookActor :
    IGuildChannelWebhookActor,
    IActor<ulong, IChannelFollowerWebhook>;
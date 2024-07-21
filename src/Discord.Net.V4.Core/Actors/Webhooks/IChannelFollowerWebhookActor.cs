using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IChannelFollowerWebhookActor :
    IWebhookActor,
    IGuildRelationship,
    IChannelRelationship<IIntegrationChannelActor, IIntegrationChannel>,
    IActor<ulong, IChannelFollowerWebhook>;

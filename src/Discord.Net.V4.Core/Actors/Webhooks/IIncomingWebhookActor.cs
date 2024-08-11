using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IIncomingWebhookActor :
    IWebhookActor,
    IGuildRelationship,
    IChannelRelationship<IIntegrationChannelTrait, IIntegrationChannel>,
    IActor<ulong, IIncomingWebhook>;

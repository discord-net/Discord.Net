using Discord.Rest;

namespace Discord;

[Creatable<CreateWebhookProperties>(nameof(Routes.CreateChannelWebhook))]
public partial interface IGuildChannelWebhookActor :
    IWebhookActor,
    IGuildRelationship,
    IChannelRelationship<IIntegrationChannelTrait, IIntegrationChannel>;
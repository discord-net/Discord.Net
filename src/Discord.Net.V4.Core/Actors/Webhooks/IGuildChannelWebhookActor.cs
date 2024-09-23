using Discord.Rest;

namespace Discord;

[Creatable<CreateWebhookProperties>(nameof(Routes.CreateChannelWebhook))]
public partial interface IGuildChannelWebhookActor :
    IWebhookActor,
    IGuildRelationship,
    IChannelRelationship<IIntegrationChannelTrait, IIntegrationChannel>
{
    [LinkExtension]
    private interface WithIncomingExtension
    {
        [LinkMirror]
        IIncomingWebhookActor Incoming { get; }
    }
    
    [LinkExtension]
    private interface WithChannelFollowerExtension
    {
        [LinkMirror]
        IChannelFollowerWebhookActor ChannelFollower { get; }
    }
}
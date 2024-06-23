using Discord.Models;

namespace Discord.Rest.Messages;

[ExtendInterfaceDefaults(typeof(IWebhookMessageActor))]
public partial class RestWebhookMessageActor(DiscordRestClient client, ulong? guildId, ulong channelId, ulong id) :
    RestMessageActor(client, guildId, channelId, id),
    IWebhookMessageActor;

public sealed partial class RestWebhookMessage(
    DiscordRestClient client,
    ulong? guildId,
    IMessageModel model,
    RestWebhookMessageActor? actor = null
):
    RestMessage(client, guildId, model, actor),
    IWebhookMessage,
    IContextConstructable<RestWebhookMessage, IMessageModel, ulong?, DiscordRestClient>
{
    [ProxyInterface(typeof(IWebhookMessageActor))]
    internal override RestWebhookMessageActor Actor { get; } = actor ?? new(client, guildId, model.ChannelId, model.Id);

    public new static RestWebhookMessage Construct(DiscordRestClient client, IMessageModel model, ulong? context)
        => new(client, context, model);
}

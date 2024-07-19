using Discord.Models;

namespace Discord.Rest.Messages;

[ExtendInterfaceDefaults(typeof(IWebhookMessageActor))]
public partial class RestWebhookMessageActor(
    DiscordRestClient client,
    MessageChannelIdentity channel,
    MessageIdentity message,
    GuildIdentity? guild = null
):
    RestMessageActor(client, channel, message, guild),
    IWebhookMessageActor
{
    public IWebhookActor Webhook => throw new NotImplementedException();
}

public sealed partial class RestWebhookMessage :
    RestMessage,
    IWebhookMessage,
    IConstructable<RestWebhookMessage, IMessageModel, DiscordRestClient>,
    IContextConstructable<RestWebhookMessage, IMessageModel, GuildIdentity?, DiscordRestClient>
{
    [ProxyInterface(typeof(IWebhookMessageActor))]
    internal override RestWebhookMessageActor Actor { get; }

    internal RestWebhookMessage(
        DiscordRestClient client,
        IMessageModel model,
        RestWebhookMessageActor? actor = null,
        GuildIdentity? guild = null,
        MessageChannelIdentity? channel = null
    ) : base(client, model, actor, guild, channel)
    {
        Actor = actor ?? new(
            client,
            channel ?? MessageChannelIdentity.Of(model.ChannelId),
            MessageIdentity.Of(this),
            guild
        );
    }

    public new static RestWebhookMessage Construct(DiscordRestClient client, GuildIdentity? guild, IMessageModel model)
        => new(client, model, guild: guild);

    public new static RestWebhookMessage Construct(DiscordRestClient client, IMessageModel model)
        => new(client, model);
}

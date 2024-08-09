using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults(typeof(IWebhookMessageActor))]
public partial class RestWebhookMessageActor :
    RestMessageActor,
    IWebhookMessageActor
{
    [SourceOfTruth] public RestWebhookActor Webhook { get; }

    internal override MessageIdentity Identity { get; }

    public RestWebhookMessageActor(
        DiscordRestClient client,
        MessageChannelIdentity channel,
        MessageIdentity message,
        WebhookIdentity webhook,
        GuildIdentity? guild = null
    ) : base(client, channel, message, guild)
    {
        Identity = message | this;

        Webhook = webhook.Actor ?? new(client, webhook);
    }

    [SourceOfTruth]
    internal override RestWebhookMessage CreateEntity(IMessageModel model)
        => RestWebhookMessage.Construct(
            Client,
            new(GuildIdentity, Channel.Identity, Webhook.Identity),
            model
        );
}

public sealed partial class RestWebhookMessage :
    RestMessage,
    IWebhookMessage,
    IConstructable<RestWebhookMessage, IMessageModel, DiscordRestClient>,
    IContextConstructable<RestWebhookMessage, IMessageModel, RestWebhookMessage.Context, DiscordRestClient>
{
    public new readonly record struct Context(
        GuildIdentity? Guild = null,
        MessageChannelIdentity? Channel = null,
        WebhookIdentity? Webhook = null
    );

    [ProxyInterface(typeof(IWebhookMessageActor))]
    internal override RestWebhookMessageActor Actor { get; }

    internal RestWebhookMessage(
        DiscordRestClient client,
        IMessageModel model,
        RestWebhookMessageActor? actor = null,
        GuildIdentity? guild = null,
        MessageChannelIdentity? channel = null,
        WebhookIdentity? webhook = null
    ) : base(client, model, actor, guild, channel)
    {
        Actor = actor ?? new(
            client,
            channel ?? MessageChannelIdentity.Of(model.ChannelId),
            MessageIdentity.Of(this),
            webhook ?? WebhookIdentity.Of(model.WebhookId ?? model.AuthorId),
            guild
        );
    }

    public static RestWebhookMessage Construct(DiscordRestClient client, Context context, IMessageModel model)
        => new(client, model, guild: context.Guild, channel: context.Channel, webhook: context.Webhook);

    public new static RestWebhookMessage Construct(DiscordRestClient client, IMessageModel model)
        => new(client, model);
}

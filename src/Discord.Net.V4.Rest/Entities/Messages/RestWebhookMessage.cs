using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults(typeof(IWebhookMessageActor))]
public partial class RestWebhookMessageActor :
    RestMessageActor,
    IWebhookMessageActor,
    IRestActor<RestWebhookMessageActor, ulong, RestWebhookMessage, IMessageModel>
{
    [SourceOfTruth] public RestWebhookActor Webhook { get; }

    public string Token { get; }

    [SourceOfTruth] internal override WebhookMessageIdentity Identity { get; }

    [TypeFactory(LastParameter = nameof(message))]
    public RestWebhookMessageActor(
        DiscordRestClient client,
        MessageChannelIdentity channel,
        WebhookIdentity webhook,
        WebhookMessageIdentity message,
        string token,
        GuildIdentity? guild = null
    ) : base(client, channel, message, guild)
    {
        Identity = message | this;
        Token = token;
        Webhook = webhook.Actor ?? new(client, webhook);
    }

    [SourceOfTruth]
    internal override RestWebhookMessage CreateEntity(IMessageModel model)
        => RestWebhookMessage.Construct(
            Client,
            this,
            model
        );
}

public sealed partial class RestWebhookMessage :
    RestMessage,
    IWebhookMessage,
    IRestConstructable<RestWebhookMessage, RestWebhookMessageActor, IMessageModel>
{
    [SourceOfTruth]
    public override RestWebhookActor Webhook => Actor.Webhook;

    [ProxyInterface(typeof(IWebhookMessageActor))]
    internal override RestWebhookMessageActor Actor { get; }

    internal RestWebhookMessage(
        DiscordRestClient client,
        IMessageModel model,
        RestWebhookMessageActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
    }

    public static RestWebhookMessage Construct(
        DiscordRestClient client,
        RestWebhookMessageActor actor,
        IMessageModel model
    ) => new(client, model, actor);
}
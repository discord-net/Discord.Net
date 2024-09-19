using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestWebhookActor :
    RestActor<RestWebhookActor, ulong, RestWebhook, IWebhookModel>,
    IWebhookActor
{
    internal override WebhookIdentity Identity { get; }

    [TypeFactory]
    public RestWebhookActor(
        DiscordRestClient client,
        WebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;
    }

    [SourceOfTruth]
    internal virtual RestWebhook CreateEntity(IWebhookModel model)
        => RestWebhook.Construct(Client, model);

    [SourceOfTruth]
    internal RestWebhookMessage CreateEntity(IMessageModel model)
        => RestWebhookMessage.Construct(Client, new(Webhook: Identity.MostSpecific(this)), model);
}

public partial class RestWebhook :
    RestEntity<ulong>,
    IWebhook,
    IRestConstructable<RestWebhook, RestWebhookActor, IWebhookModel>
{
    public WebhookType Type => (WebhookType)Model.Type;

    [SourceOfTruth] public RestUserActor? Creator { get; private set; }

    public string? Name => Model.Name;

    public string? Avatar => Model.Avatar;

    public ulong? ApplicationId => Model.ApplicationId;

    [ProxyInterface(typeof(IWebhookActor))]
    internal virtual RestWebhookActor Actor { get; }

    internal IWebhookModel Model { get; private set; }

    internal RestWebhook(
        DiscordRestClient client,
        IWebhookModel model,
        RestWebhookActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, WebhookIdentity.Of(this));
        Model = model;
    }

    public static RestWebhook Construct(DiscordRestClient client, IWebhookModel model)
    {
        return (WebhookType)model.Type switch
        {
            WebhookType.Incoming when model is {GuildId: not null, ChannelId: not null} =>
                RestIncomingWebhook.Construct(
                    client,
                    new(
                        GuildIdentity.Of(model.GuildId.Value),
                        TextChannelIdentity.Of(model.ChannelId.Value)
                    ), model
                ),
            WebhookType.ChannelFollower when model is {GuildId: not null, ChannelId: not null} =>
                RestChannelFollowerWebhook.Construct(
                    client,
                    new(
                        GuildIdentity.Of(model.GuildId.Value),
                        TextChannelIdentity.Of(model.ChannelId.Value)
                    ),
                    model
                ),
            _ => new RestWebhook(client, model)
        };
    }

    public virtual ValueTask UpdateAsync(IWebhookModel model, CancellationToken token = default)
    {
        Creator = Creator.UpdateFrom(
            model.UserId,
            RestUserActor.Factory,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IWebhookModel GetModel() => Model;
}

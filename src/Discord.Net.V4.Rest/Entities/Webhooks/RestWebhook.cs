using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds;

namespace Discord.Rest.Webhooks;

[method: TypeFactory]
public partial class RestWebhookActor(
    DiscordRestClient client,
    WebhookIdentity webhook
) :
    RestActor<ulong, RestWebhook, WebhookIdentity>(client, webhook),
    IWebhookActor
{
    [SourceOfTruth]
    internal virtual RestWebhook CreateEntity(IWebhookModel model)
        => RestWebhook.Construct(Client, model);
}

public partial class RestWebhook :
    RestEntity<ulong>,
    IWebhook,
    IConstructable<RestWebhook, IWebhookModel, DiscordRestClient>
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

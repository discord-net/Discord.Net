using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayWebhookActor :
    GatewayCachedActor<ulong, GatewayWebhook, WebhookIdentity, IWebhookModel>,
    IWebhookActor
{
    internal override WebhookIdentity Identity { get; }

    public GatewayWebhookActor(
        DiscordGatewayClient client,
        WebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;
    }

    public IWebhookMessage CreateEntity(IMessageModel model) => throw new NotImplementedException();
    public IWebhook CreateEntity(IWebhookModel model) => throw new NotImplementedException();
}

[ExtendInterfaceDefaults]
public partial class GatewayWebhook :
    GatewayCacheableEntity<GatewayWebhook, ulong, IWebhookModel>,
    IWebhook
{
    public WebhookType Type => (WebhookType)Model.Type;

    [SourceOfTruth]
    public GatewayUserActor? Creator { get; private set; }

    public string? Name => Model.Name;

    public string? Avatar => Model.Avatar;

    public ulong? ApplicationId => Model.ApplicationId;

    [ProxyInterface] internal virtual GatewayWebhookActor Actor { get; }

    internal virtual IWebhookModel Model => _model;

    private IWebhookModel _model;

    public GatewayWebhook(
        DiscordGatewayClient client,
        IWebhookModel model,
        GatewayWebhookActor? actor = null
    ) : base(client, model.Id)
    {
        _model = model;

        Actor = actor ?? new(client, WebhookIdentity.Of(this));

        UpdateLinkedActors(model);
    }

    public static GatewayWebhook Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IWebhookModel model)
    {
        // TODO: switch type
    }

    private void UpdateLinkedActors(IWebhookModel model)
    {
        Creator = Creator.UpdateFrom(
            model.UserId,
            (client, user) => client.Users[user],
            Client
        );
    }

    public override ValueTask UpdateAsync(
        IWebhookModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        UpdateLinkedActors(model);

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IWebhookModel GetModel() => Model;

}

using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayIncomingWebhookActor :
    GatewayWebhookActor,
    IIncomingWebhookActor,
    IGatewayCachedActor<ulong, GatewayIncomingWebhook, IncomingWebhookIdentity, IWebhookModel>
{
    [SourceOfTruth] public GatewayGuildActor Guild { get; }

    [SourceOfTruth] public GatewayIncomingIntegrationChannelTrait Channel { get; }

    [SourceOfTruth] internal override IncomingWebhookIdentity Identity { get; }

    public GatewayIncomingWebhookActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IncomingIntegrationChannelIdentity channel,
        IncomingWebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;

        Guild = client.Guilds[guild];
        Channel = new(client, Guild.Channels[channel.Id], channel);
    }

    [SourceOfTruth]
    internal override GatewayIncomingWebhook CreateEntity(IWebhookModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

[ExtendInterfaceDefaults]
public sealed partial class GatewayIncomingWebhook :
    GatewayWebhook,
    IIncomingWebhook,
    ICacheableEntity<GatewayIncomingWebhook, ulong, IWebhookModel>
{
    public string? Token => Model.Token;

    public string? Url => Model.Url;

    [ProxyInterface] internal override GatewayIncomingWebhookActor Actor { get; }

    internal GatewayIncomingWebhook(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IncomingIntegrationChannelIdentity channel,
        IWebhookModel model,
        GatewayIncomingWebhookActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, guild, channel, IncomingWebhookIdentity.Of(this));
    }

    public new static GatewayIncomingWebhook Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IWebhookModel model
    ) => new(
        client,
        context.Path.RequireIdentity(Template.T<GuildIdentity>()),
        context.Path.RequireIdentity(Template.T<IncomingIntegrationChannelIdentity>()),
        model,
        context.TryGetActor<GatewayIncomingWebhookActor>()
    );

    ValueTask IUpdatable<IWebhookModel>.UpdateAsync(IWebhookModel model, CancellationToken token)
        => UpdateAsync(model, true, token);
}

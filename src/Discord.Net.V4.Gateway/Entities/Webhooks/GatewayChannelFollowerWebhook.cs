using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayChannelFollowerWebhookActor :
    GatewayWebhookActor,
    IChannelFollowerWebhookActor,
    IGatewayCachedActor<ulong, GatewayChannelFollowerWebhook, ChannelFollowerWebhookIdentity, IWebhookModel>
{
    [SourceOfTruth] public GatewayGuildActor Guild { get; }

    [SourceOfTruth] public GatewayChannelFollowerIntegrationChannelTrait Channel { get; }

    [SourceOfTruth] internal override ChannelFollowerWebhookIdentity Identity { get; }

    public GatewayChannelFollowerWebhookActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ChannelFollowerIntegrationChannelIdentity channel,
        ChannelFollowerWebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;

        Guild = client.Guilds[guild];
        Channel = new(client, Guild.Channels[channel.Id], channel);
    }

    [SourceOfTruth]
    internal override GatewayChannelFollowerWebhook CreateEntity(IWebhookModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayChannelFollowerWebhook :
    GatewayWebhook,
    ICacheableEntity<GatewayChannelFollowerWebhook, ulong, IWebhookModel>,
    IChannelFollowerWebhook
{
    [SourceOfTruth] public GatewayGuildActor? SourceGuild { get; private set; }

    public string? SourceGuildIcon => Model.SourceGuildIcon;

    public string? SourceGuildName => Model.SourceGuildName;

    [SourceOfTruth] public GatewayNewsChannelActor? SourceChannel { get; private set; }

    public string? SourceChannelName => Model.SourceChannelName;

    [ProxyInterface] internal override GatewayChannelFollowerWebhookActor Actor { get; }

    public GatewayChannelFollowerWebhook(
        DiscordGatewayClient client,
        GuildIdentity guild,
        ChannelFollowerIntegrationChannelIdentity channel,
        IWebhookModel model,
        GatewayChannelFollowerWebhookActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, guild, channel, ChannelFollowerWebhookIdentity.Of(this));

        UpdateLinkedActors(model);
    }

    public new static GatewayChannelFollowerWebhook Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IWebhookModel model
    ) => new(
        client,
        context.Path.RequireIdentity(Template.T<GuildIdentity>()),
        context.Path.RequireIdentity(Template.T<ChannelFollowerIntegrationChannelIdentity>()),
        model,
        context.TryGetActor<GatewayChannelFollowerWebhookActor>()
    );

    private void UpdateLinkedActors(IWebhookModel model)
    {
        SourceGuild = SourceGuild.UpdateFrom(
            model.SourceGuildId,
            (client, guild) => client.Guilds[guild],
            Client
        );

        SourceChannel = SourceChannel.UpdateFrom(
            model.SourceChannelId,
            (guild, channel) => guild?.AnnouncementChannels[channel]!,
            SourceGuild
        );
    }

    public override ValueTask UpdateAsync(
        IWebhookModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        UpdateLinkedActors(model);

        return base.UpdateAsync(model, false, token);
    }

    ValueTask IUpdatable<IWebhookModel>.UpdateAsync(IWebhookModel model, CancellationToken token)
        => UpdateAsync(model, true, token);
}

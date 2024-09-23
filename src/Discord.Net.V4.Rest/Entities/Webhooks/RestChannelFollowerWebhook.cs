using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestChannelFollowerWebhookActor :
    RestGuildChannelWebhookActor,
    IChannelFollowerWebhookActor,
    IRestActor<RestChannelFollowerWebhookActor, ulong, RestChannelFollowerWebhook, IChannelFollowerWebhookModel>
{
    public override IRestIntegrationChannelTrait.WithChannelFollower Channel { get; }

    [SourceOfTruth] internal override ChannelFollowerWebhookIdentity Identity { get; }

    [TypeFactory]
    public RestChannelFollowerWebhookActor(
        DiscordRestClient client,
        GuildIdentity guild,
        ChannelFollowerIntegrationChannelIdentity channel,
        ChannelFollowerWebhookIdentity webhook
    ) : base(client, guild, channel, webhook)
    {
        Identity = webhook | this;

        Channel =
            channel.Actor ?? base.Channel as IRestIntegrationChannelTrait.WithChannelFollower ??
            IRestIntegrationChannelTrait.WithChannelFollower.GetContainerized(Guild.Channels[channel.Id]);
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestChannelFollowerWebhook CreateEntity(IChannelFollowerWebhookModel model)
        => RestChannelFollowerWebhook.Construct(Client, this, model);
}

public sealed partial class RestChannelFollowerWebhook :
    RestWebhook,
    IChannelFollowerWebhook,
    IRestConstructable<RestChannelFollowerWebhook, RestChannelFollowerWebhookActor, IChannelFollowerWebhookModel>
{
    [SourceOfTruth]
    public RestGuildActor? SourceGuild
        => Model.SourceGuildId.HasValue ? Client.Guilds[Model.SourceGuildId.Value] : null;

    public string? SourceGuildIcon => Model.SourceGuildIcon;

    public string? SourceGuildName => Model.SourceGuildName;

    [SourceOfTruth] public RestNewsChannelActor? SourceChannel 
        => Model.SourceChannelId.HasValue ? SourceGuild?.Channels.News[Model.SourceChannelId.Value] : null;

    public string? SourceChannelName => Model.SourceChannelName;

    [ProxyInterface(typeof(IChannelFollowerWebhookActor))]
    internal override RestChannelFollowerWebhookActor Actor { get; }

    internal override IChannelFollowerWebhookModel Model => _model;

    private IChannelFollowerWebhookModel _model;

    internal RestChannelFollowerWebhook(
        DiscordRestClient client,
        IChannelFollowerWebhookModel model,
        RestChannelFollowerWebhookActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestChannelFollowerWebhook Construct(
        DiscordRestClient client,
        RestChannelFollowerWebhookActor actor,
        IChannelFollowerWebhookModel model)
        => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IChannelFollowerWebhookModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IChannelFollowerWebhookModel GetModel() => Model;
}
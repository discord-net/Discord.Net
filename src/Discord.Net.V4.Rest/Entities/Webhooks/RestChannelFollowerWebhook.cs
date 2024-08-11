using Discord.Models;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestChannelFollowerWebhookActor :
    RestWebhookActor,
    IChannelFollowerWebhookActor,
    IRestActor<ulong, RestChannelFollowerWebhook, WebhookIdentity>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public RestChannelFollowerIntegrationChannelTrait Channel { get; }

    [SourceOfTruth]
    internal override WebhookIdentity Identity { get; }

    [TypeFactory]
    public RestChannelFollowerWebhookActor(
        DiscordRestClient client,
        GuildIdentity guild,
        ChannelFollowerIntegrationChannelIdentity channel,
        WebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;

        Guild = new RestGuildActor(client, guild);
        Channel = new(client, Guild.Channels[channel.Id], channel);
    }

    [SourceOfTruth]
    internal override RestChannelFollowerWebhook CreateEntity(IWebhookModel model)
        => RestChannelFollowerWebhook.Construct(Client, new(Guild.Identity, Channel.Identity), model);
}

public sealed partial class RestChannelFollowerWebhook :
    RestWebhook,
    IChannelFollowerWebhook,
    IContextConstructable<RestChannelFollowerWebhook, IWebhookModel, RestChannelFollowerWebhook.Context,
        DiscordRestClient>
{
    public readonly record struct Context(GuildIdentity Guild, ChannelFollowerIntegrationChannelIdentity Channel);

    [SourceOfTruth] public RestGuildActor? SourceGuild { get; private set; }

    public string? SourceGuildIcon => Model.SourceGuildIcon;

    public string? SourceGuildName => Model.SourceGuildName;

    [SourceOfTruth] public RestNewsChannelActor? SourceChannel { get; private set; }

    public string? SourceChannelName => Model.SourceChannelName;

    [ProxyInterface(typeof(IChannelFollowerWebhookActor))]
    internal override RestChannelFollowerWebhookActor Actor { get; }

    internal RestChannelFollowerWebhook(
        DiscordRestClient client,
        GuildIdentity guild,
        ChannelFollowerIntegrationChannelIdentity channel,
        IWebhookModel model,
        RestChannelFollowerWebhookActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, guild, channel, WebhookIdentity.Of(this));

        SourceGuild = model.SourceGuildId.Map(
            static (id, client) => new RestGuildActor(client, GuildIdentity.Of(id)),
            client
        );

        SourceChannel = model.SourceChannelId.Map(
            static (id, client, guild) =>
                guild is null
                    ? null
                    : new RestNewsChannelActor(client, guild, NewsChannelIdentity.Of(id)),
            client,
            SourceGuild?.Identity
        );
    }

    public static RestChannelFollowerWebhook Construct(DiscordRestClient client, Context context, IWebhookModel model)
        => new(client, context.Guild, context.Channel, model);

    public override ValueTask UpdateAsync(IWebhookModel model, CancellationToken token = default)
    {
        SourceGuild = SourceGuild.UpdateFrom(
            model.SourceGuildId,
            RestGuildActor.Factory,
            Client
        );

        SourceChannel = SourceChannel.UpdateFrom(
            model.SourceChannelId,
            static (client, guild, channel) => guild is null
                ? null!
                : RestNewsChannelActor.Factory(client, guild, channel),
            Client,
            SourceGuild?.Identity
        );

        return base.UpdateAsync(model, token);
    }
}

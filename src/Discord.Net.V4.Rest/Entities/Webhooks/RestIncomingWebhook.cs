using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Guilds;

namespace Discord.Rest.Webhooks;

[method: TypeFactory]
public sealed partial class RestIncomingWebhookActor(
    DiscordRestClient client,
    GuildIdentity guild,
    IntegrationChannelIdentity channel,
    WebhookIdentity webhook
):
    RestWebhookActor(client, webhook),
    IIncomingWebhookActor,
    IRestActor<ulong, RestIncomingWebhook, WebhookIdentity>
{
    [SourceOfTruth] public RestGuildActor Guild { get; } = new(client, guild);

    [SourceOfTruth] public RestIntegrationChannelActor Channel { get; } = new(client, guild, channel);

    [SourceOfTruth]
    internal override RestIncomingWebhook CreateEntity(IWebhookModel model)
        => RestIncomingWebhook.Construct(Client, new(Guild.Identity, Channel.IntegrationChannelIdentity), model);
}

public sealed partial class RestIncomingWebhook :
    RestWebhook,
    IIncomingWebhook,
    IContextConstructable<RestIncomingWebhook, IWebhookModel, RestIncomingWebhook.Context, DiscordRestClient>
{
    public readonly record struct Context(GuildIdentity Guild, IntegrationChannelIdentity Channel);

    public string? Token => Model.Token;

    public string? Url => Model.Url;

    [ProxyInterface(typeof(IIncomingWebhookActor))]
    internal override RestIncomingWebhookActor Actor { get; }

    internal RestIncomingWebhook(
        DiscordRestClient client,
        GuildIdentity guild,
        IntegrationChannelIdentity channel,
        IWebhookModel model,
        RestIncomingWebhookActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, guild, channel, WebhookIdentity.Of(this));
    }

    public static RestIncomingWebhook Construct(DiscordRestClient client, Context context, IWebhookModel model)
        => new(client, context.Guild, context.Channel, model);
}

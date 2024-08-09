using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestIncomingWebhookActor :
    RestWebhookActor,
    IIncomingWebhookActor,
    IRestActor<ulong, RestIncomingWebhook, WebhookIdentity>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public RestIntegrationChannelActor Channel { get; }

    [SourceOfTruth] internal override WebhookIdentity Identity { get; }

    [TypeFactory]
    public RestIncomingWebhookActor(DiscordRestClient client,
        GuildIdentity guild,
        IntegrationChannelIdentity channel,
        WebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;

        Guild = new RestGuildActor(client, guild);
        Channel = new RestIntegrationChannelActor(client, guild, channel);
    }

    [SourceOfTruth]
    internal override RestIncomingWebhook CreateEntity(IWebhookModel model)
        => RestIncomingWebhook.Construct(Client, new(Guild.Identity, Channel.Identity), model);
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

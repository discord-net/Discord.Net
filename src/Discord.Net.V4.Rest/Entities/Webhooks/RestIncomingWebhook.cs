using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestIncomingWebhookActor :
    RestWebhookActor,
    IIncomingWebhookActor,
    IRestActor<ulong, RestIncomingWebhook, WebhookIdentity>
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    [SourceOfTruth] public RestIncomingIntegrationChannelTrait Channel { get; }

    [SourceOfTruth] internal override WebhookIdentity Identity { get; }

    [TypeFactory]
    public RestIncomingWebhookActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IncomingIntegrationChannelIdentity channel,
        WebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;

        Guild = new RestGuildActor(client, guild);
        Channel = new RestIncomingIntegrationChannelTrait(client, Guild.Channels[channel.Id], channel);
    }

    [SourceOfTruth]
    internal override RestIncomingWebhook CreateEntity(IWebhookModel model)
        => RestIncomingWebhook.Construct(Client, new(Guild.Identity, Channel.Identity), model);
}

public sealed partial class RestIncomingWebhook :
    RestWebhook,
    IIncomingWebhook,
    IRestConstructable<RestIncomingWebhook, RestIncomingWebhookActor, IWebhookModel>
{
    public readonly record struct Context(GuildIdentity Guild, IncomingIntegrationChannelIdentity Channel);

    public string? Token => Model.Token;

    public string? Url => Model.Url;

    [ProxyInterface(typeof(IIncomingWebhookActor))]
    internal override RestIncomingWebhookActor Actor { get; }

    internal RestIncomingWebhook(
        DiscordRestClient client,
        GuildIdentity guild,
        IncomingIntegrationChannelIdentity channel,
        IWebhookModel model,
        RestIncomingWebhookActor? actor = null
    ) : base(client, model, actor)
    {
        Actor = actor ?? new(client, guild, channel, WebhookIdentity.Of(this));
    }

    public static RestIncomingWebhook Construct(DiscordRestClient client, Context context, IWebhookModel model)
        => new(client, context.Guild, context.Channel, model);
}

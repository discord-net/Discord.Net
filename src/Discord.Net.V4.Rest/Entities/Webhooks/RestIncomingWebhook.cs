using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestIncomingWebhookActor :
    RestGuildChannelWebhookActor,
    IIncomingWebhookActor,
    IRestActor<RestIncomingWebhookActor, ulong, RestIncomingWebhook, IIncomingWebhookModel>
{
    public override IRestIntegrationChannelTrait Channel => base.Channel;

    [SourceOfTruth] internal override IncomingWebhookIdentity Identity { get; }

    [TypeFactory]
    public RestIncomingWebhookActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IncomingIntegrationChannelIdentity channel,
        IncomingWebhookIdentity webhook
    ) : base(client, guild, channel, webhook)
    {
        Identity = webhook | this;
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestIncomingWebhook CreateEntity(IIncomingWebhookModel model)
        => RestIncomingWebhook.Construct(Client, this, model);
}

public sealed partial class RestIncomingWebhook :
    RestWebhook,
    IIncomingWebhook,
    IRestConstructable<RestIncomingWebhook, RestIncomingWebhookActor, IIncomingWebhookModel>
{
    public string? Token => Model.Token;

    public string? Url => Model.Url;

    [ProxyInterface(typeof(IIncomingWebhookActor))]
    internal override RestIncomingWebhookActor Actor { get; }

    internal override IIncomingWebhookModel Model => _model;

    private IIncomingWebhookModel _model;

    internal RestIncomingWebhook(
        DiscordRestClient client,
        IIncomingWebhookModel model,
        RestIncomingWebhookActor actor
    ) : base(client, model, actor)
    {
        Actor = actor;
        _model = model;
    }

    public static RestIncomingWebhook Construct(
        DiscordRestClient client, 
        RestIncomingWebhookActor actor,
        IIncomingWebhookModel model
        ) => new(client, model, actor);

    public override IWebhookModel GetModel() => Model;
}

using Discord.Models;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestIncomingWebhookActor :
    RestGuildChannelWebhookActor,
    IIncomingWebhookActor,
    IRestActor<RestIncomingWebhookActor, ulong, RestIncomingWebhook, IIncomingWebhookModel>
{
    public WithToken this[string token]
        => _withTokenCache.GetOrAdd(token, (self, token) => new(self, token), this);
    
    public override IRestIntegrationChannelTrait.WithIncoming Channel { get; }

    [SourceOfTruth] internal override IncomingWebhookIdentity Identity { get; }

    private readonly WeakDictionary<string, WithToken> _withTokenCache = new();

    [TypeFactory]
    public RestIncomingWebhookActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IncomingIntegrationChannelIdentity channel,
        IncomingWebhookIdentity webhook
    ) : base(client, guild, channel, webhook)
    {
        Identity = webhook | this;
        Channel =
            channel.Actor ?? IRestIntegrationChannelTrait.WithIncoming.GetContainerized(Guild.Channels[channel.Id]);
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestIncomingWebhook CreateEntity(IIncomingWebhookModel model)
        => RestIncomingWebhook.Construct(Client, this, model);

    IIncomingWebhookWithTokenActor IIncomingWebhookActor.this[string token] => this[token];
    
    public partial class WithToken : RestIncomingWebhookActor, IIncomingWebhookWithTokenActor
    {
        [SourceOfTruth] public RestWebhookMessageActor.Indexable Messages { get; }

        public string Token { get; }

        public WithToken(
            RestIncomingWebhookActor actor,
            string token
        ) : base(
            actor.Client, 
            actor.Guild.Identity, 
            IncomingIntegrationChannelIdentity.Of(actor.Channel),
            actor.Identity)
        {
            Token = token;

            Messages = new(
                Client,
                RestActorProvider.GetOrCreate(
                    Client,
                    Template.Of<WebhookMessageIdentity>(),
                    MessageChannelIdentity.Of((IRestMessageChannelTrait) actor.Channel),
                    (WebhookIdentity) actor.Identity,
                    token
                )
            );
        }
    }
}

public sealed partial class RestIncomingWebhook :
    RestWebhook,
    IIncomingWebhook,
    IRestConstructable<RestIncomingWebhook, RestIncomingWebhookActor, IIncomingWebhookModel>,
    IRestConstructable<RestIncomingWebhook, RestIncomingWebhookActor.WithToken, IIncomingWebhookModel>
{
    public string Token => Model.Token;

    public string? Url => Model.Url;

    [ProxyInterface(typeof(IIncomingWebhookWithTokenActor))]
    internal override RestIncomingWebhookActor.WithToken Actor { get; }

    internal override IIncomingWebhookModel Model => _model;

    private IIncomingWebhookModel _model;

    internal RestIncomingWebhook(
        DiscordRestClient client,
        IIncomingWebhookModel model,
        RestIncomingWebhookActor.WithToken actor
    ) : base(client, model, actor)
    {
        Actor = actor;
        _model = model;
    }

    public static RestIncomingWebhook Construct(
        DiscordRestClient client,
        RestIncomingWebhookActor actor,
        IIncomingWebhookModel model
    ) => Construct(client, actor[model.Token], model);
    
    public static RestIncomingWebhook Construct(
        DiscordRestClient client,
        RestIncomingWebhookActor.WithToken actor,
        IIncomingWebhookModel model
    ) => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IIncomingWebhookModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IIncomingWebhookModel GetModel() => Model;
}
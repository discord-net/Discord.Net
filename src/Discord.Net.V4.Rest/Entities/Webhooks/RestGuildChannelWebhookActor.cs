namespace Discord.Rest;

public abstract partial class RestGuildChannelWebhookActor :
    RestWebhookActor,
    IGuildChannelWebhookActor
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; }

    [SourceOfTruth]
    public virtual IRestIntegrationChannelTrait Channel { get; }

    internal override GuildChannelWebhookIdentity Identity { get; }

    [TypeFactory]
    protected RestGuildChannelWebhookActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IntegrationChannelIdentity channel,
        GuildChannelWebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook;

        Guild = client.Guilds[guild];
        Channel = channel.Actor ?? IRestIntegrationChannelTrait.GetContainerized(
            Guild.Channels[channel.Id]
        );
    }
}
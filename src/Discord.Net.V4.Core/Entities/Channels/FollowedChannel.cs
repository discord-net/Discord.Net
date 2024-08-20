namespace Discord;

public readonly struct FollowedChannel(IDiscordClient client, ulong channelId, ulong webhookId) :
    IModelConstructable<FollowedChannel, Models.Json.FollowedChannel>
{
    public readonly IChannelActor Channel = client.Channel(channelId);
    public readonly IWebhookActor Webhook = client.Webhook(webhookId);

    public static FollowedChannel Construct(IDiscordClient client, Models.Json.FollowedChannel model) =>
        new(client, model.ChannelId, model.WebhookId);
}

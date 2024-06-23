namespace Discord;

public readonly struct FollowedChannel(IDiscordClient client, ulong channelId, ulong webhookId) :
    IConstructable<FollowedChannel, Models.Json.FollowedChannel>
{
    public readonly ILoadableChannelActor Channel = client.Channel(channelId);
    public readonly ILoadableWebhookActor Webhook = client.Webhook(webhookId);

    public static FollowedChannel Construct(IDiscordClient client, Models.Json.FollowedChannel model) =>
        new(client, model.ChannelId, model.WebhookId);
}

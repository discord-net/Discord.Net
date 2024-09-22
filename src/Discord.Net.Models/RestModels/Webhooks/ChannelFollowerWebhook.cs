using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 2)]
public sealed class ChannelFollowerWebhook : Webhook, IChannelFollowerWebhookModel
{
    [JsonPropertyName("source_guild")]
    public required PartialGuild SourceGuild { get; set; }

    [JsonPropertyName("source_channel")]
    public required WebhookSourceChannel SourceChannel { get; set; }

    ulong IChannelFollowerWebhookModel.SourceGuildId => SourceGuild.Id;
    string IChannelFollowerWebhookModel.SourceGuildName => SourceGuild.Name;
    string? IChannelFollowerWebhookModel.SourceGuildIcon => ~SourceGuild.Icon;
    ulong IChannelFollowerWebhookModel.SourceChannelId => SourceChannel.Id;
    string IChannelFollowerWebhookModel.SourceChannelName => SourceChannel.Name;
}
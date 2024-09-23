using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 2)]
public sealed class ChannelFollowerWebhook : Webhook, IChannelFollowerWebhookModel
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("source_guild")]
    public Optional<PartialGuild>  SourceGuild { get; set; }

    [JsonPropertyName("source_channel")]
    public Optional<WebhookSourceChannel> SourceChannel { get; set; }

    ulong? IChannelFollowerWebhookModel.SourceGuildId => SourceGuild.Map(v => v.Id).ToNullable();
    string? IChannelFollowerWebhookModel.SourceGuildName => ~SourceGuild.Map(v => v.Name);
    string? IChannelFollowerWebhookModel.SourceGuildIcon => ~SourceGuild.Map(v => v.Icon);
    ulong? IChannelFollowerWebhookModel.SourceChannelId => SourceChannel.Map(v => v.Id).ToNullable();
    string? IChannelFollowerWebhookModel.SourceChannelName => ~SourceChannel.Map(v => v.Name);
}
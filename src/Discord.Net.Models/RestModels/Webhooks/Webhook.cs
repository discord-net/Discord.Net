using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Webhook : IWebhookModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong?> GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> Creator { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("source_guild")]
    public Optional<PartialGuild> SourceGuild { get; set; }

    [JsonPropertyName("source_channel")]
    public Optional<Channel> SourceChannel { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    ulong? IWebhookModel.GuildId => GuildId;
    ulong? IWebhookModel.SourceGuildId => SourceGuild.Map(v => v.Id);
    ulong? IWebhookModel.SourceChannelId => SourceChannel.Map(v => v.Id);
    string? IWebhookModel.Url => Url;
    ulong? IWebhookModel.UserId => Creator.Map(v => v.Id);
}

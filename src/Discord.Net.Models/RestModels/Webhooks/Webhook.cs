using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class WebhookSourceChannel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

public sealed class Webhook : IWebhookModel, IModelSource, IModelSourceOf<IUserModel?>
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
    public Optional<WebhookSourceChannel> SourceChannel { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    ulong? IWebhookModel.GuildId => ~GuildId;
    ulong? IWebhookModel.SourceGuildId => ~SourceGuild.Map(v => v.Id);
    string? IWebhookModel.SourceGuildName => ~SourceGuild.Map(v => v.Name);
    string? IWebhookModel.SourceGuildIcon => ~SourceGuild.Map(v => v.Icon);
    ulong? IWebhookModel.SourceChannelId => ~SourceChannel.Map(v => v.Id);
    string? IWebhookModel.SourceChannelName => ~SourceChannel.Map(v => v.Name);
    string? IWebhookModel.Url => ~Url;
    ulong? IWebhookModel.UserId => ~Creator.Map(v => v.Id);

    public IEnumerable<IEntityModel> GetDefinedModels()
    {
        if (Creator) yield return Creator.Value;
    }

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~Creator;
}

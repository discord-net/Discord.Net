using Newtonsoft.Json;

namespace Discord.API;

internal class Webhook
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public WebhookType Type { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong?> GuildId { get; set; }

    [JsonProperty("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonProperty("user")]
    public Optional<User> Creator { get; set; }

    [JsonProperty("name")]
    public Optional<string> Name { get; set; }

    [JsonProperty("avatar")]
    public Optional<string> Avatar { get; set; }

    [JsonProperty("token")]
    public Optional<string> Token { get; set; }

    [JsonProperty("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonProperty("source_guild")]
    public Optional<PartialGuild> Guild { get; set; }

    [JsonProperty("source_channel")]
    public Optional<Channel> Channel { get; set; }
}

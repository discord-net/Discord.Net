using System.Text.Json.Serialization;

namespace Discord.Models
{
    public class Webhook
    {
        public const int MinWebhookNameLength = 2;
        public const int MaxWebhookNameLength = 32;

        public const int MinMessageContentLength = 0;
        public const int MaxMessageContentLength = 2000;

        public const int MinEmbedLimit = 0;
        public const int MaxEmbedLimit = 10;

        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }
        [JsonPropertyName("type")]
        public WebhookType Type { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }
        [JsonPropertyName("user")]
        public Optional<User> Creator { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("avatar")]
        public string? AvatarId { get; set; }
        [JsonPropertyName("token")]
        public Optional<string> Token { get; set; }
    }
}

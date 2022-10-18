using System.Text.Json.Serialization;

namespace Discord.API
{
    [JsonConverter(typeof(Net.Converters.InteractionConverter))]
    internal class Interaction
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonPropertyName("type")]
        public InteractionType Type { get; set; }

        [JsonPropertyName("data")]
        public Optional<IDiscordInteractionData> Data { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<ulong> ChannelId { get; set; }

        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; set; }

        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("message")]
        public Optional<Message> Message { get; set; }

        [JsonPropertyName("locale")]
        public Optional<string> UserLocale { get; set; }

        [JsonPropertyName("guild_locale")]
        public Optional<string> GuildLocale { get; set; }
    }
}

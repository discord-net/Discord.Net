using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class IntegrationDeletedEvent
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("application_id")]
        public Optional<ulong> ApplicationID { get; set; }
    }
}

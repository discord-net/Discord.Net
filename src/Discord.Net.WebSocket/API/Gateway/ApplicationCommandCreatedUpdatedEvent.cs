using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class ApplicationCommandCreatedUpdatedEvent : ApplicationCommand
    {
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }
    }
}

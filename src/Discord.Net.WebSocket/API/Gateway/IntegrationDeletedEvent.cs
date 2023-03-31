using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class IntegrationDeletedEvent
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("application_id")]
        public Optional<ulong> ApplicationID { get; set; }
    }
}

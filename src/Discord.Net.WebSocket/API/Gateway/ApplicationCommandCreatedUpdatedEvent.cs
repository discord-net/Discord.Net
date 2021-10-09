using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class ApplicationCommandCreatedUpdatedEvent : ApplicationCommand
    {
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
    }
}

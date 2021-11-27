using Newtonsoft.Json;

namespace Discord.API
{
    internal class ApplicationCommand
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("type")]
        public ApplicationCommandType Type { get; set; } = ApplicationCommandType.Slash; // defaults to 1 which is slash.

        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermissions { get; set; }
    }
}

using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyApplicationCommandParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }

        [JsonProperty("description")]
        public Optional<string> Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }
    }
}

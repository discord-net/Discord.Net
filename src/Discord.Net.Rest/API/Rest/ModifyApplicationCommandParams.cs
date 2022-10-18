using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Discord.API.Rest
{
    internal class ModifyApplicationCommandParams
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }
    }
}

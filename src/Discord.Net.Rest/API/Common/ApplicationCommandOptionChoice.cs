using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommandOptionChoice
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonPropertyName("name_localized")]
        public Optional<string> NameLocalized { get; set; }
    }
}

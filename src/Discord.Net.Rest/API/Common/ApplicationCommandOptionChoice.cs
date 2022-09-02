using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommandOptionChoice
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonProperty("name_localized")]
        public Optional<string> NameLocalized { get; set; }
    }
}

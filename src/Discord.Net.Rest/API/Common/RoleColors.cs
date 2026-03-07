using Newtonsoft.Json;

namespace Discord.API
{
    internal class RoleColors
    {
        [JsonProperty("primary_color")]
        public Optional<uint?> PrimaryColor { get; set; }

        [JsonProperty("secondary_color")]
        public Optional<uint?> SecondaryColor { get; set; }

        [JsonProperty("tertiary_color")]
        public Optional<uint?> TertiaryColor { get; set; }
    }
}

using Newtonsoft.Json;

namespace Discord.API
{
    internal class RoleColors
    {
        [JsonProperty("primary_color")]
        public uint PrimaryColor { get; set; }

        [JsonProperty("secondary_color")]
        public uint? SecondaryColor { get; set; }

        [JsonProperty("tertiary_color")]
        public uint? TertiaryColor { get; set; }
    }
}

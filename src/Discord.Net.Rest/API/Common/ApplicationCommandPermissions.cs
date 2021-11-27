using Newtonsoft.Json;

namespace Discord.API
{
    internal class ApplicationCommandPermissions
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("type")]
        public ApplicationCommandPermissionTarget Type { get; set; }

        [JsonProperty("permission")]
        public bool Permission { get; set; }
    }
}

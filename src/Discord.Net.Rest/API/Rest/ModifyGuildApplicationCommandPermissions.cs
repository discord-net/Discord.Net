using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyGuildApplicationCommandPermissions
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; }
    }
}

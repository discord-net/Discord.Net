using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyGuildApplicationCommandPermissionsParams
    {
        [JsonProperty("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; }
    }
}

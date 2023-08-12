using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyGuildApplicationCommandPermissionsParams
    {
        [JsonPropertyName("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; }
    }
}

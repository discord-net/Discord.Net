using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildApplicationCommandPermissions
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; }
    }
}

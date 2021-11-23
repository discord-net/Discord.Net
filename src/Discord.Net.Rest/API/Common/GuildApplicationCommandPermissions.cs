using Newtonsoft.Json;

namespace Discord.API
{
    internal class GuildApplicationCommandPermission
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; }
    }
}

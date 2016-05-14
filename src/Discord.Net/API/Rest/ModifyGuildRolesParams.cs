using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        [JsonProperty("id")]
        public Optional<ulong> Id { get; set; }
    }
}

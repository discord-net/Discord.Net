using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
    }
}

#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        [JsonProperty("id")]
        public ulong Id { internal get; set; }
    }
}

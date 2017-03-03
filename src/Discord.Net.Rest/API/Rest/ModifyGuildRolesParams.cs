#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        [JsonProperty("id")]
        public ulong Id { get; }

        public ModifyGuildRolesParams(ulong id)
        {
            Id = id;
        }
    }
}

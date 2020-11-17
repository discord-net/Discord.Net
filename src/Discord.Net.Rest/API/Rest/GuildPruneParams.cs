#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class GuildPruneParams
    {
        [JsonProperty("days")]
        public int Days { get; }

        [JsonProperty("include_roles")]
        public ulong[] IncludeRoleIds { get; }

        public GuildPruneParams(int days, ulong[] includeRoleIds)
        {
            Days = days;
            IncludeRoleIds = includeRoleIds;
        }
    }
}

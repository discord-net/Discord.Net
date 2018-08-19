#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class GuildPruneParams
    {
        public GuildPruneParams(int days)
        {
            Days = days;
        }

        [JsonProperty("days")] public int Days { get; }
    }
}

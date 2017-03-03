#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildChannelsParams
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }

        public ModifyGuildChannelsParams(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}

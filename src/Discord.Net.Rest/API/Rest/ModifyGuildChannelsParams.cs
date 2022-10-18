using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildChannelsParams
    {
        [JsonPropertyName("id")]
        public ulong Id { get; }
        [JsonPropertyName("position")]
        public int Position { get; }

        public ModifyGuildChannelsParams(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}

using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildChannelParams
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
        [JsonPropertyName("position")]
        public Optional<int> Position { get; set; }
        [JsonPropertyName("parent_id")]
        public Optional<ulong?> CategoryId { get; set; }
        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]> Overwrites { get; set; }
        [JsonPropertyName("flags")]
        public Optional<ChannelFlags?> Flags { get; set; }
    }
}

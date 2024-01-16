using Newtonsoft.Json;

namespace Discord.API
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyForumTagParams
    {
        [JsonProperty("id")]
        public Optional<ulong> Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("emoji_id")]
        public Optional<ulong?> EmojiId { get; set; }

        [JsonProperty("emoji_name")]
        public Optional<string> EmojiName { get; set; }

        [JsonProperty("moderated")]
        public bool Moderated { get; set; }
    }
}

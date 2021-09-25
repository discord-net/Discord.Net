using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyMessageParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }
        [JsonProperty("embeds")]
        public Optional<API.Embed[]> Embeds { get; set; }
        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }
        [JsonProperty("flags")]
        public Optional<MessageFlags?> Flags { get; set; }
        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }
    }
}

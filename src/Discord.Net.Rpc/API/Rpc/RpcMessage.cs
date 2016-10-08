using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class RpcMessage : Message
    {
        [JsonProperty("blocked")]
        public Optional<bool> IsBlocked { get; }
        [JsonProperty("content_parsed")]
        public Optional<object[]> ContentParsed { get; }
        [JsonProperty("author_color")]
        public Optional<string> AuthorColor { get; } //#Hex

        [JsonProperty("mentions")]
        public new Optional<ulong[]> UserMentions { get; set; }
    }
}

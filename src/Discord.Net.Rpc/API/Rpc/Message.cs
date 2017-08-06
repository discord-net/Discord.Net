using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class Message : Discord.API.Message
    {
        [ModelProperty("blocked")]
        public Optional<bool> IsBlocked { get; }
        [ModelProperty("content_parsed")]
        public Optional<object[]> ContentParsed { get; }
        [ModelProperty("author_color")]
        public Optional<string> AuthorColor { get; } //#Hex

        [ModelProperty("mentions")]
        public new Optional<ulong[]> UserMentions { get; set; }
    }
}

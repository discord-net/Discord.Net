using Discord.Serialization;

namespace Discord.API
{
    internal class Reaction
    {
        [ModelProperty("count")]
        public int Count { get; set; }
        [ModelProperty("me")]
        public bool Me { get; set; }
        [ModelProperty("emoji")]
        public Emoji Emoji { get; set; }
    }
}

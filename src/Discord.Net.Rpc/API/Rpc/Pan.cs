using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class Pan
    {
        [ModelProperty("left")]
        public float Left { get; set; }
        [ModelProperty("right")]
        public float Right { get; set; }
    }
}

#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Game
    {
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("url")]
        public Optional<string> StreamUrl { get; set; }
        [ModelProperty("type")]
        public Optional<StreamType?> StreamType { get; set; }
    }
}

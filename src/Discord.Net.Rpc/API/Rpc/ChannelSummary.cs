using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class ChannelSummary
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("type")]
        public ChannelType Type { get; set; }
    }
}

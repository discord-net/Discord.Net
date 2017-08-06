#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class GetBotGatewayResponse
    {
        [ModelProperty("url")]
        public string Url { get; set; }
        [ModelProperty("shards")]
        public int Shards { get; set; }
    }
}

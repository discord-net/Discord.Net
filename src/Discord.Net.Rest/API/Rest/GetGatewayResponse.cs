#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class GetGatewayResponse
    {
        [ModelProperty("url")]
        public string Url { get; set; }
    }
}

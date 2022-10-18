using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    internal class GetGatewayResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}

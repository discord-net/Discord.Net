using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class GetGatewayRequest : IRestRequest<GetGatewayResponse>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"gateway";
        object IRestRequest.Payload => null;
    }
    
    public class GetGatewayResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

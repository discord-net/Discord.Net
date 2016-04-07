using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GatewayRequest : IRestRequest<GatewayResponse>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"gateway";
        object IRestRequest.Payload => null;
    }

    public class GatewayResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

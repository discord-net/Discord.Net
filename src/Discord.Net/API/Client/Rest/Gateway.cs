using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GatewayRequest : IRestRequest<GatewayResponse>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"gateway";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;
    }
    
    public sealed class GatewayResponse
	{
		[JsonProperty("url")]
		public string Url { get; set; }
    }
}

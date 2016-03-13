using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetVoiceRegionsRequest : IRestRequest<GetVoiceRegionsResponse[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"voice/regions";
        object IRestRequest.Payload => null;
    }
    
    public class GetVoiceRegionsResponse
    {
        [JsonProperty("sample_hostname")]
        public string Hostname { get; set; }
        [JsonProperty("sample_port")]
        public int Port { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("vip")]
        public bool Vip { get; set; }
    }
}

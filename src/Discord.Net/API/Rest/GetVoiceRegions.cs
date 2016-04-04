using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class GetVoiceRegionsRequest : IRestRequest<GetVoiceRegionsResponse[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"voice/regions";
        object IRestRequest.Payload => null;
    }
    
    public class GetVoiceRegionsResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("vip")]
        public bool IsVip { get; set; }
        [JsonProperty("optimal")]
        public bool IsOptimal { get; set; }
        [JsonProperty("sample_hostname")]
        public string SampleHostname { get; set; }
        [JsonProperty("sample_port")]
        public int SamplePort { get; set; }
    }
}

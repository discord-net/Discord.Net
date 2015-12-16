using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetVoiceRegionsRequest : IRestRequest<GetVoiceRegionsResponse[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/voice/regions";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;
    }
    
    public sealed class GetVoiceRegionsResponse
    {
        [JsonProperty("sample_hostname")]
        public string Hostname { get; set; }
        [JsonProperty("sample_port")]
        public int Port { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

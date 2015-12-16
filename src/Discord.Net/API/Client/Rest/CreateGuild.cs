using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CreateGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/guilds";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("icon")]
        public string IconBase64 { get; set; }
    }
}

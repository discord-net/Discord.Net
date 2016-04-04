using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateMessageRequest : IRestRequest<Message>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; }

        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
        public string Nonce { get; set; } = null;
        [JsonProperty("tts", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsTTS { get; set; } = false;

        public CreateMessageRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

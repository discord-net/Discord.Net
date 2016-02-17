using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SendMessageRequest : IRestRequest<Message>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
        public string Nonce { get; set; }
        [JsonProperty("tts")]
        public bool IsTTS { get; set; }

        public SendMessageRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

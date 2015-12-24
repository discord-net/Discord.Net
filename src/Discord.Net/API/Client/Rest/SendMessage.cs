using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SendMessageRequest : IRestRequest<Message>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }

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

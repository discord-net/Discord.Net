using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateMessageRequest : IRestRequest<Message>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; }
        public ulong MessageId { get; }

        [JsonProperty("content")]
        public string Content { get; set; } = "";

        public UpdateMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

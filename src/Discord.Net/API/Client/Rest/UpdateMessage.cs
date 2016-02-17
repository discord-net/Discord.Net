using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateMessageRequest : IRestRequest<Message>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; } = "";

        public UpdateMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

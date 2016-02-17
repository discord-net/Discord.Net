using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DeleteMessageRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        public DeleteMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

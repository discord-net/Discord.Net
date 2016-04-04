using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteMessageRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }
        public ulong MessageId { get; }

        public DeleteMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

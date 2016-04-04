using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class AckMessageRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}/ack";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }
        public ulong MessageId { get; }

        public AckMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class TriggerTypingIndicatorRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/typing";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }

        public TriggerTypingIndicatorRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

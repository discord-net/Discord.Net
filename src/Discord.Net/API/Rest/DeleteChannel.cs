using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"channels/{ChannelId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }

        public DeleteChannelRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

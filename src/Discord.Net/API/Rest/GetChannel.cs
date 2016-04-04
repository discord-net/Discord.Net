using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"channels/{ChannelId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }

        public GetChannelRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

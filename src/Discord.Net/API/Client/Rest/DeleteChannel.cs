using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class DeleteChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"channels/{ChannelId}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; set; }

        public DeleteChannelRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

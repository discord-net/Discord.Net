using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class BroadcastTypingRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/channels/{ChannelId}/typing";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }

        public BroadcastTypingRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

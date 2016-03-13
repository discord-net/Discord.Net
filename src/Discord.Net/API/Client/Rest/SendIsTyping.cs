using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SendIsTypingRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/typing";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; set; }

        public SendIsTypingRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

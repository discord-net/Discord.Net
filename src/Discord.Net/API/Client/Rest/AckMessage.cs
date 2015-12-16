using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AckMessageRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/channels/{ChannelId}/messages/{MessageId}/ack";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }
        public ulong MessageId { get; }

        /*[JsonProperty("manual")]
        public bool Manual { get; set; }*/

        public AckMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

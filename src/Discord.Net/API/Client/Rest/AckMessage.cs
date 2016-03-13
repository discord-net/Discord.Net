using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AckMessageRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}/ack";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        /*[JsonProperty("manual")]
        public bool Manual { get; set; }*/

        public AckMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}

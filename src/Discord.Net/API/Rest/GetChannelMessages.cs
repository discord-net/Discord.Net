using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetChannelMessagesRequest : IRestRequest<Message[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages?limit={Limit}&{RelativeDir}={RelativeId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }

        public Relative RelativeDir { get; set; }
        public ulong RelativeId { get; set; } = 0;

        [JsonProperty("limit")]
        public int Limit { get; set; } = 100;

        [JsonProperty("before")]
        public ulong Before => RelativeId;
        private bool ShouldSerializeBefore => RelativeDir == Relative.Before;

        [JsonProperty("after")]
        public ulong After => RelativeId;
        private bool ShouldSerializeAfter => RelativeDir == Relative.After;

        public GetChannelMessagesRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

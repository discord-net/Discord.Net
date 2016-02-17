using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"channels/{ChannelId}";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }

        public UpdateChannelRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

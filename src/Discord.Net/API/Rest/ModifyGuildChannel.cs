using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyGuildChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"channels/{ChannelId}";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }

        public ModifyGuildChannelRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

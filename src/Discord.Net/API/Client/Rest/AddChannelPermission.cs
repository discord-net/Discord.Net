using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AddOrUpdateChannelPermissionsRequest : IRestRequest
    {
        string IRestRequest.Method => "PUT";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/permissions/{TargetId}";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; set; }

        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong TargetId { get; set; }
        [JsonProperty("type")]
        public string TargetType { get; set; }
        [JsonProperty("allow")]
        public uint Allow { get; set; }
        [JsonProperty("deny")]
        public uint Deny { get; set; }

        public AddOrUpdateChannelPermissionsRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

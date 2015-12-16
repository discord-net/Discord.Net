using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AddChannelPermissionsRequest : IRestRequest
    {
        string IRestRequest.Method => "PUT";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/permissions";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }

        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong TargetId { get; set; }
        [JsonProperty("type")]
        public string TargetType { get; set; }
        [JsonProperty("allow")]
        public uint Allow { get; set; }
        [JsonProperty("deny")]
        public uint Deny { get; set; }

        public AddChannelPermissionsRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

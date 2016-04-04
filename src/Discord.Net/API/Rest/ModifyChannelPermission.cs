using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyChannelPermissionsRequest : IRestRequest
    {
        string IRestRequest.Method => "PUT";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/permissions/{TargetId}";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; }        
        public ulong TargetId { get; }

        [JsonProperty("allow")]
        public uint Allow { get; set; }
        [JsonProperty("deny")]
        public uint Deny { get; set; }

        public ModifyChannelPermissionsRequest(ulong channelId, ulong targetId)
        {
            ChannelId = channelId;
            TargetId = targetId;
        }
    }
}

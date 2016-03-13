using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RemoveChannelPermissionsRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/permissions/{TargetId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; set; }
        public ulong TargetId { get; set; }

        public RemoveChannelPermissionsRequest(ulong channelId, ulong targetId)
        {
            ChannelId = channelId;
            TargetId = targetId;
        }
    }
}

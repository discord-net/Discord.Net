using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RemoveChannelPermissionsRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/channels/{ChannelId}/permissions/{TargetId}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }
        public ulong TargetId { get; }

        public RemoveChannelPermissionsRequest(ulong channelId, ulong targetId)
        {
            ChannelId = channelId;
            TargetId = targetId;
        }
    }
}

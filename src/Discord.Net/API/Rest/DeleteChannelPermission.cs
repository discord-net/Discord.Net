using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteChannelPermissionsRequest : IRestRequest
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/permissions/{TargetId}";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }
        public ulong TargetId { get; }

        public DeleteChannelPermissionsRequest(ulong channelId, ulong targetId)
        {
            ChannelId = channelId;
            TargetId = targetId;
        }
    }
}

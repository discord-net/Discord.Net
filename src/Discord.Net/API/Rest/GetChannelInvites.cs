using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetChannelInvitesRequest : IRestRequest<InviteMetadata[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/invites";
        object IRestRequest.Payload => null;

        public ulong ChannelId { get; }        

        public GetChannelInvitesRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}

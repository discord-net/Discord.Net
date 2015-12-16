using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AcceptInviteRequest : IRestRequest<InviteReference>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/invite/{InviteId}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public string InviteId { get; }

        public AcceptInviteRequest(string inviteId)
        {
            InviteId = inviteId;
        }
    }
}

using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class DeleteInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/invite/{InviteCode}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public string InviteCode { get; }

        public DeleteInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

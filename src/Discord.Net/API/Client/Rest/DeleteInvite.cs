using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DeleteInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"invite/{InviteCode}";
        object IRestRequest.Payload => null;

        public string InviteCode { get; set; }

        public DeleteInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

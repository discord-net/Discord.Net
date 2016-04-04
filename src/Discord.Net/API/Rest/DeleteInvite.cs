using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class DeleteInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "DELETE";
        string IRestRequest.Endpoint => $"invites/{InviteCode}";
        object IRestRequest.Payload => null;

        public string InviteCode { get; }

        public DeleteInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class AcceptInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"invites/{InviteCode}";
        object IRestRequest.Payload => null;

        public string InviteCode { get; }

        public AcceptInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

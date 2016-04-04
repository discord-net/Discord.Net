using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"invites/{InviteCode}";
        object IRestRequest.Payload => null;

        public string InviteCode { get; }

        public GetInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

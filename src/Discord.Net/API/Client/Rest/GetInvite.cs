using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetInviteRequest : IRestRequest<InviteReference>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"invite/{InviteCode}";
        object IRestRequest.Payload => null;

        public string InviteCode { get; set; }

        public GetInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

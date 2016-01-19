using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AcceptInviteRequest : IRestRequest<InviteReference>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"invite/{InviteId}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public string InviteId { get; set; }

        public AcceptInviteRequest(string inviteId)
        {
            InviteId = inviteId;
        }
    }
}

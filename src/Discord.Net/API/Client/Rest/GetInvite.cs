using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetInviteRequest : IRestRequest<InviteReference>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/invite/{InviteCode}";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public string InviteCode { get; }

        public GetInviteRequest(string inviteCode)
        {
            InviteCode = inviteCode;
        }
    }
}

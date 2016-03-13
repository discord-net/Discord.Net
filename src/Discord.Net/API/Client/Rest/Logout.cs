using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LogoutRequest : IRestRequest
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"auth/logout";
        object IRestRequest.Payload => null;
    }
}

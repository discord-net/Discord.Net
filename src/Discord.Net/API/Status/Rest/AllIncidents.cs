using Newtonsoft.Json;

namespace Discord.API.Status.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetAllIncidentsRequest : IRestRequest<StatusResult>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"incidents.json";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;
    }
}

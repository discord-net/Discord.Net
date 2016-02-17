using Newtonsoft.Json;

namespace Discord.API.Status.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetActiveMaintenancesRequest : IRestRequest<StatusResult>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"scheduled-maintenances/active.json";
        object IRestRequest.Payload => null;
    }
}

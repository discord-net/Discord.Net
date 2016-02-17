using Newtonsoft.Json;

namespace Discord.API.Status.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GetUpcomingMaintenancesRequest : IRestRequest<StatusResult>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"scheduled-maintenances/upcoming.json";
        object IRestRequest.Payload => null;
    }
}

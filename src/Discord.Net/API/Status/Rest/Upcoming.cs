using Newtonsoft.Json;

namespace Discord.API.Status.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetUpcomingIncidentsRequest : IRestRequest<Incident>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"{DiscordConfig.StatusAPIUrl}/scheduled-maintenances/upcoming.json";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;
    }
}

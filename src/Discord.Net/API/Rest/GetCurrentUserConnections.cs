using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetCurrentUserConnectionsRequest : IRestRequest<Connection[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"users/@me/connections";
        object IRestRequest.Payload => null;
    }
}

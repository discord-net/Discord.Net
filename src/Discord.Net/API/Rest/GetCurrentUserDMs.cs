using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetCurrentUserDMsRequest : IRestRequest<Channel[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"users/@me/channels";
        object IRestRequest.Payload => null;
    }
}

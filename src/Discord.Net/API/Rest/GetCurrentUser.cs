using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetCurrentUserRequest : IRestRequest<User>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"users/@me";
        object IRestRequest.Payload => null;
    }
}

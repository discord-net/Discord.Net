using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetCurrentUserGuildsRequest : IRestRequest<Guild[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"users/@me/guilds";
        object IRestRequest.Payload => null;
    }
}

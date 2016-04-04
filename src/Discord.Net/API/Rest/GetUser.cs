using Discord.Net.Rest;

namespace Discord.API.Rest
{
    public class GetUserRequest : IRestRequest<User>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"users/{UserId}";
        object IRestRequest.Payload => null;

        public ulong UserId { get; }

        public GetUserRequest(ulong userId)
        {
            UserId = userId;
        }
    }
}

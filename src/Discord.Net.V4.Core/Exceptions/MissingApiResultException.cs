namespace Discord;

public class MissingApiResultException(ApiRoute route, RequestOptions requestOptions, Exception? inner = null)
    : DiscordException($"Error executing {route.Name}", inner)
{
    public ApiRoute Route { get; } = route;
    public RequestOptions RequestOptions { get; } = requestOptions;
}

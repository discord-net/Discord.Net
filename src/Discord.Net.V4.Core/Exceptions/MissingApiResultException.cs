namespace Discord;

public class MissingApiResultException(IApiRoute route, RequestOptions requestOptions, Exception? inner = null)
    : DiscordException($"Error executing {route.Name}", inner)
{
    public IApiRoute Route { get; } = route;
    public RequestOptions RequestOptions { get; } = requestOptions;
}

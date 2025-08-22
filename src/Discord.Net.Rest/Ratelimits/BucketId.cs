using Discord.Rest.Api;

namespace Discord.Rest.Ratelimits;

public readonly record struct BucketId(
    string Route,
    Snowflake? GuildId = null,
    Snowflake? ChannelId = null,
    Snowflake? WebhookId = null,
    string? WebhookToken = null
)
{
    public static BucketId FromRoute<T>(T route) where T : IRoute
        => new(
            T.Path,
            route.RouteParameters.OfType<RouteParameters.GuildId>().FirstOrDefault()?.Value,
            route.RouteParameters.OfType<RouteParameters.ChannelId>().FirstOrDefault()?.Value,
            route.RouteParameters.OfType<RouteParameters.WebhookId>().FirstOrDefault()?.Value,
            route.RouteParameters.OfType<RouteParameters.WebhookToken>().FirstOrDefault()?.Value
        );
}
using Discord.API;
using Discord.Utils;

namespace Discord;

public static partial class Routes
{
    public static APIRoute<User> GetUser(ulong userId)
        => new(nameof(GetUser), RequestMethod.Get, $"users/{userId}");

    public static readonly APIRoute<User> CurrentUser
        = new(nameof(CurrentUser), RequestMethod.Get, "users/@me");

    public static APIBodyRoute<SelfUserProperties, User> ModifyCurrentUser(SelfUserProperties properties)
        => new(nameof(ModifyCurrentUser), RequestMethod.Patch, "users/@me", properties);

    public static APIRoute<PartialGuild> GetCurrentUserGuilds(ulong? before, ulong? after, int? limit, bool? withCounts)
        => new(
            nameof(GetCurrentUserGuilds),
            RequestMethod.Get,
            $"users/@me/guilds{RouteUtils.GetUrlEncodedQueryParams(("before", before), ("after", after), ("limit", limit), ("with_counts", withCounts))}"
        );

    public static APIRoute<>
}

using System.Net;

namespace Discord;

/// <summary>
///     Generates route strings for Discord API endpoints.
/// </summary>
public class Routes
{
    public static string GetUrlEncodedQueryParams(params (string, object?)[] args)
    {
        if (args.All(x => x.Item2 is null))
            return string.Empty;

        var paramsString = string.Join("&", args.Where(x => x.Item2 is not null)
            .Select(x => GetUrlEncodedQueryParam(x.Item1, x.Item2!)));

        return $"?{paramsString}";
    }

    public static string GetUrlEncodedQueryParam(string key, object value)
        => $"{key}={WebUtility.UrlEncode(value.ToString())}";

    #region User

    public static string GetUser(ulong userId)
        => $"{DiscordConfig.APIUrl}users/{userId}";

    public static string CurrentUser
        => $"{DiscordConfig.APIUrl}users/@me";

    public static string GetCurrentUserGuilds(ulong? before = default, ulong? after = default, int? limit = default, bool? withCounts = default)
        => $"{DiscordConfig.APIUrl}users/@me/guilds{GetUrlEncodedQueryParams(("before", before),
            ("after", after),
            ("limit", limit),
            ("with_counts", withCounts))}";

    public static string GetCurrentUserGuildMember(ulong guildId)
        => $"{DiscordConfig.APIUrl}users/@me/guilds/{guildId}/member";

    public static string LeaveGuild(ulong guildId)
        => $"{DiscordConfig.APIUrl}users/@me/guilds/{guildId}";

    public static string CreateDm
        => "{DiscordConfig.APIUrl}users/@me/channels";

    public static string GetUserConnections
        => "{DiscordConfig.APIUrl}users/@me/connections";

    public static string ApplicationRoleConnection(ulong applicationId)
        => $"{DiscordConfig.APIUrl}users/@me/applications/{applicationId}/role-connection";

    #endregion
}

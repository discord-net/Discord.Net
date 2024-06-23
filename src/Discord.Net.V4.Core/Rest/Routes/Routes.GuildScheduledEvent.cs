using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<GuildScheduledEvent[]> ListGuildScheduledEvents(ulong guildId, bool? withUserCount) =>
        new ApiOutRoute<GuildScheduledEvent[]>(nameof(ListGuildScheduledEvents), RequestMethod.Get,
            $"guilds/{guildId}/scheduled-events{RouteUtils.GetUrlEncodedQueryParams(("with_user_count", withUserCount))}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildScheduledEvent> GetGuildScheduledEvent(ulong guildId, ulong eventId) =>
        new ApiOutRoute<GuildScheduledEvent>(nameof(GetGuildScheduledEvent), RequestMethod.Get,
            $"/guilds/{guildId}/scheduled-events/{eventId}", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateGuildScheduledEventParams, GuildScheduledEvent> CreateGuildScheduledEvent(
        ulong guildId, CreateGuildScheduledEventParams body) =>
        new ApiInOutRoute<CreateGuildScheduledEventParams, GuildScheduledEvent>(nameof(CreateGuildScheduledEvent),
            RequestMethod.Post, $"/guilds/{guildId}/scheduled-events", body, ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildScheduledEventParams, GuildScheduledEvent> ModifyGuildScheduledEvent(
        ulong guildId, ulong eventId, ModifyGuildScheduledEventParams body) =>
        new ApiInOutRoute<ModifyGuildScheduledEventParams, GuildScheduledEvent>(nameof(ModifyGuildScheduledEvent),
            RequestMethod.Patch, $"/guilds/{guildId}/scheduled-events/{eventId}", body, ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildScheduledEvent(ulong guildId, ulong eventId) =>
        new ApiRoute(nameof(DeleteGuildScheduledEvent), RequestMethod.Delete,
            $"/guilds/{guildId}/scheduled-events/{eventId}", (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildScheduledEventUser[]> GetGuildScheduledEventUsers(ulong guildId, ulong eventId,
        int? limit = default,
        bool? withMember = default, ulong? beforeId = default, ulong? afterId = default) =>
        new ApiOutRoute<GuildScheduledEventUser[]>(nameof(GetGuildScheduledEventUsers), RequestMethod.Get,
            $"/guilds/{guildId}/scheduled-events/{eventId}/users{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("with_member", withMember), ("before", beforeId), ("after", afterId))}",
            (ScopeType.Guild, guildId));
}

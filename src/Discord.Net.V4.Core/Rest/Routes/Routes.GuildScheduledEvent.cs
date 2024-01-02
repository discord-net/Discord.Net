using Discord.API;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<GuildScheduledEvent[]> ListGuildScheduledEvents(ulong guildId, bool? withUserCount)
        => new(nameof(ListGuildScheduledEvents),
            RequestMethod.Get,
            $"guilds/{guildId}/scheduled-events{RouteUtils.GetUrlEncodedQueryParams(("with_user_count", withUserCount))}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildScheduledEvent> GetGuildScheduledEvent(ulong guildId, ulong eventId)
        => new(nameof(GetGuildScheduledEvent),
            RequestMethod.Get,
            $"/guilds/{guildId}/scheduled-events/{eventId}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateGuildScheduledEventParams, GuildScheduledEvent> CreateGuildScheduledEvent(ulong guildId, CreateGuildScheduledEventParams body)
        => new(nameof(CreateGuildScheduledEvent),
            RequestMethod.Post,
            $"/guilds/{guildId}/scheduled-events",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildScheduledEventParams, GuildScheduledEvent> ModifyGuildScheduledEvent(ulong guildId, ulong eventId, ModifyGuildScheduledEventParams body)
        => new(nameof(ModifyGuildScheduledEvent),
            RequestMethod.Patch,
            $"/guilds/{guildId}/scheduled-events/{eventId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteGuildScheduledEvent(ulong guildId, ulong eventId)
        => new(nameof(DeleteGuildScheduledEvent),
            RequestMethod.Delete,
            $"/guilds/{guildId}/scheduled-events/{eventId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildScheduledEventUser[]> GetGuildScheduledEventUsers(ulong guildId, ulong eventId, int? limit = default,
        bool? withMember = default, ulong? beforeId = default, ulong? afterId = default)
        => new(nameof(GetGuildScheduledEventUsers),
            RequestMethod.Get,
            $"/guilds/{guildId}/scheduled-events/{eventId}/users{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("with_member", withMember), ("before", beforeId), ("after", afterId))}",
            (ScopeType.Guild, guildId));
}

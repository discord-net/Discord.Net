using Discord.API;
using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<Invite> GetInvite(string code, bool? withCounts = default, bool? withExpiration = default, ulong? eventId = default)
        => new(nameof(GetInvite),
            RequestMethod.Get,
            $"invites/{code}{RouteUtils.GetUrlEncodedQueryParams(("with_counts", withCounts), ("with_expiration", withExpiration), ("guild_scheduled_event_id", eventId))}");

    public static ApiRoute<Invite> DeleteInvite(string code)
        => new(nameof(DeleteInvite),
            RequestMethod.Delete,
            $"invites/{code}");
}

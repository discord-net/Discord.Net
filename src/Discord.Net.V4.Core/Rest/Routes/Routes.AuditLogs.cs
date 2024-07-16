using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<AuditLog> GetAuditLog(
        [IdHeuristic<IGuild>] ulong guildId
    ) => new ApiOutRoute<AuditLog>(
        nameof(GetAuditLog),
        RequestMethod.Get,
        $"guilds/{guildId}/audit-logs",
        (ScopeType.Guild, guildId)
    );
}

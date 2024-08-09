using Discord.Models.Json;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly IApiOutRoute<VoiceRegion[]> ListVoiceRegions
        = new ApiOutRoute<VoiceRegion[]>(nameof(ListVoiceRegions), RequestMethod.Get, "voice/regions");

    public static IApiOutRoute<VoiceState> GetCurrentUserVoiceState(
        [IdHeuristic<IGuild>] ulong guildId
    ) => new ApiOutRoute<VoiceState>(
        nameof(GetCurrentUserVoiceState),
        RequestMethod.Get,
        $"guilds/{guildId}/voice-states/@me",
        (ScopeType.Guild, guildId)
    );

    public static IApiOutRoute<VoiceState> GetUserVoiceState(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IUser>, IdHeuristic<IVoiceState>] ulong userId
    ) => new ApiOutRoute<VoiceState>(
        nameof(GetCurrentUserVoiceState),
        RequestMethod.Get,
        $"guilds/{guildId}/voice-states/{userId}",
        (ScopeType.Guild, guildId)
    );

    public static IApiInRoute<ModifyCurrentUserVoiceStateParams> ModifyCurrentUserVoiceState(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyCurrentUserVoiceStateParams body
    ) => new ApiInRoute<ModifyCurrentUserVoiceStateParams>(
        nameof(ModifyCurrentUserVoiceState),
        RequestMethod.Patch,
        $"guilds/{guildId}/voice-states/@me",
        body,
        ContentType.JsonBody,
        (ScopeType.Guild, guildId)
    );

    public static IApiInRoute<ModifyUserVoiceStateParams> ModifyUserVoiceState([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IMember>] ulong userId,
        ModifyUserVoiceStateParams body
    ) => new ApiInRoute<ModifyUserVoiceStateParams>(
        nameof(ModifyUserVoiceState),
        RequestMethod.Patch,
        $"guilds/{guildId}/voice-states/{userId}",
        body,
        ContentType.JsonBody,
        (ScopeType.Guild, guildId)
    );
}

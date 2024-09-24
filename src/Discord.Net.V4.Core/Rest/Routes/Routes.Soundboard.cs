using Discord.Models;
using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiRoute SendSoundboardSound(
        [IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<ISoundboardSound>] ulong soundId,
        [IdHeuristic<IGuild>] ulong? soundGuildId = null
    ) => new ApiRoute(
        nameof(SendSoundboardSound),
        RequestMethod.Post,
        $"channels/{channelId}/send-soundboard-sound{RouteUtils.GetUrlEncodedQueryParams(
            ("sound_id", soundId),
            ("source_guild_id", soundGuildId)
        )}"
    );

    public static readonly IApiOutRoute<IEnumerable<SoundboardSound>> ListDefaultSoundboardSounds
        = new ApiOutRoute<IEnumerable<SoundboardSound>>(
            nameof(ListDefaultSoundboardSounds),
            RequestMethod.Get,
            "soundboard-default-sounds"
        );

    public static IApiOutRoute<IEnumerable<GuildSoundboardSound>> ListGuildSoundboardSounds(
        [IdHeuristic<IGuild>] ulong guildId
    ) => new ApiOutRoute<IEnumerable<GuildSoundboardSound>>(
        nameof(ListGuildSoundboardSounds),
        RequestMethod.Get,
        $"guilds/{guildId}/soundboard-sounds",
        (ScopeType.Guild, guildId)
    );

    public static IApiOutRoute<GuildSoundboardSound> GetGuildSoundboardSound(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<ISoundboardSound>] ulong soundId
    ) => new ApiOutRoute<GuildSoundboardSound>(
        nameof(GetGuildSoundboardSound),
        RequestMethod.Get,
        $"guilds/{guildId}/soundboard-sounds/{soundId}",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<CreateGuildSoundboardSoundParams, GuildSoundboardSound> CreateGuildSoundboardSound(
        [IdHeuristic<IGuild>] ulong guildId,
        CreateGuildSoundboardSoundParams args
    ) => new ApiInOutRoute<CreateGuildSoundboardSoundParams, GuildSoundboardSound>(
        nameof(CreateGuildSoundboardSound),
        RequestMethod.Post,
        $"guilds/{guildId}/soundboard-sounds",
        args,
        bucket: (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<ModifyGuildSoundboardSoundParams, GuildSoundboardSound> ModifyGuildSoundboardSound(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<ISoundboardSound>] ulong soundId,
        ModifyGuildSoundboardSoundParams args
    ) => new ApiInOutRoute<ModifyGuildSoundboardSoundParams, GuildSoundboardSound>(
        nameof(ModifyGuildSoundboardSound),
        RequestMethod.Patch,
        $"guilds/{guildId}/soundboard-sounds/{soundId}",
        args,
        bucket: (ScopeType.Guild, guildId)
    );

    public static IApiRoute DeleteGuildSoundboardSound(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<ISoundboardSound>] ulong soundId
    ) => new ApiRoute(
        nameof(DeleteGuildSoundboardSound),
        RequestMethod.Delete,
        $"guilds/{guildId}/soundboard-sounds/{soundId}",
        bucket: (ScopeType.Guild, guildId)
    );
}
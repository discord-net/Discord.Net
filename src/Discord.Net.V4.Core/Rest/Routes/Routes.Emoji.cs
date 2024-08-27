using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<CustomEmote[]> ListGuildEmojis([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<CustomEmote[]>(nameof(ListGuildEmojis), RequestMethod.Get, $"guilds/{guildId}/emojis",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<CustomEmote> GetGuildEmoji([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildEmote>] ulong emojiId) =>
        new ApiOutRoute<Models.Json.CustomEmote>(nameof(GetGuildEmoji), RequestMethod.Get,
            $"guilds/{guildId}/emojis/{emojiId}", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateGuildEmojiParams, CustomEmote>
        CreateGuildEmoji([IdHeuristic<IGuild>] ulong guildId, CreateGuildEmojiParams body) =>
        new ApiInOutRoute<CreateGuildEmojiParams, CustomEmote>(nameof(CreateGuildEmoji), RequestMethod.Post,
            $"guilds/{guildId}/emojis", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyEmojiParams, CustomEmote> ModifyGuildEmoji([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildEmote>] ulong emojiId,
        ModifyEmojiParams body) =>
        new ApiInOutRoute<ModifyEmojiParams, CustomEmote>(nameof(ModifyGuildEmoji), RequestMethod.Patch,
            $"guilds/{guildId}/emojis/{emojiId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildEmoji([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildEmote>] ulong emojiId) =>
        new ApiRoute(nameof(DeleteGuildEmoji), RequestMethod.Delete, $"guilds/{guildId}/emojis/{emojiId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<IEnumerable<CustomEmote>> ListApplicationEmojis(
        [IdHeuristic<IApplication>] ulong applicationId
    ) => new ApiOutRoute<IEnumerable<CustomEmote>>(
        nameof(ListApplicationEmojis),
        RequestMethod.Get,
        $"applications/{applicationId}/emojis"
    );

    public static IApiOutRoute<CustomEmote> GetApplicationEmoji(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IApplicationEmote>] ulong emoteId
    ) => new ApiOutRoute<CustomEmote>(
        nameof(GetApplicationEmoji),
        RequestMethod.Get,
        $"applications/{applicationId}/emojis/{emoteId}"
    );

    public static IApiInOutRoute<CreateApplicationEmojiParams, CustomEmote> CreateApplicationEmoji(
        [IdHeuristic<IApplication>] ulong applicationId,
        CreateApplicationEmojiParams body
    ) => new ApiInOutRoute<CreateApplicationEmojiParams, CustomEmote>(
        nameof(CreateApplicationEmoji),
        RequestMethod.Post,
        $"applications/{applicationId}/emojis",
        body
    );

    public static IApiInOutRoute<ModifyApplicationEmojiParams, CustomEmote> ModifyApplicationEmoji(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IApplicationEmote>] ulong emoteId,
        ModifyApplicationEmojiParams body
    ) => new ApiInOutRoute<ModifyApplicationEmojiParams, CustomEmote>(
        nameof(ModifyApplicationEmoji),
        RequestMethod.Patch,
        $"applications/{applicationId}/emojis/{emoteId}",
        body
    );

    public static IApiRoute DeleteApplicationEmoji(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IApplicationEmote>] ulong emojiId
    ) => new ApiRoute(
        nameof(DeleteApplicationEmoji),
        RequestMethod.Delete,
        $"applications/{applicationId}/emojis/{emojiId}"
    );
}
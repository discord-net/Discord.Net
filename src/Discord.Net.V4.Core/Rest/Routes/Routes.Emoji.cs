using Discord.Models.Json;

namespace Discord.Rest;

using GuildEmote = Models.Json.GuildEmote;

public partial class Routes
{
    public static IApiOutRoute<GuildEmote[]> ListGuildEmojis([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<GuildEmote[]>(nameof(ListGuildEmojis), RequestMethod.Get, $"guilds/{guildId}/emojis",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildEmote> GetGuildEmoji([IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IGuildEmote>] ulong emojiId) =>
        new ApiOutRoute<Models.Json.GuildEmote>(nameof(GetGuildEmoji), RequestMethod.Get,
            $"guilds/{guildId}/emojis/{emojiId}", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateEmojiParams, GuildEmote>
        CreateGuildEmoji([IdHeuristic<IGuild>] ulong guildId, CreateEmojiParams body) =>
        new ApiInOutRoute<CreateEmojiParams, GuildEmote>(nameof(CreateGuildEmoji), RequestMethod.Post,
            $"guilds/{guildId}/emojis", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyEmojiParams, GuildEmote> ModifyGuildEmoji([IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IGuildEmote>] ulong emojiId,
        ModifyEmojiParams body) =>
        new ApiInOutRoute<ModifyEmojiParams, GuildEmote>(nameof(ModifyGuildEmoji), RequestMethod.Patch,
            $"guilds/{guildId}/emojis/{emojiId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildEmoji([IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IGuildEmote>] ulong emojiId) =>
        new ApiRoute(nameof(DeleteGuildEmoji), RequestMethod.Delete, $"guilds/{guildId}/emojis/{emojiId}",
            (ScopeType.Guild, guildId));
}

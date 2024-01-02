using Discord.API;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<API.Emoji[]> ListGuildEmojis(ulong guildId)
        => new(nameof(ListGuildEmojis),
            RequestMethod.Get,
            $"guilds/{guildId}/emojis",
            (ScopeType.Guild, guildId));

    public static ApiRoute<API.Emoji> GetGuildEmoji(ulong guildId, ulong emojiId)
        => new(nameof(GetGuildEmoji),
        RequestMethod.Get,
        $"guilds/{guildId}/emojis/{emojiId}",
        (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateEmojiParams, API.Emoji> CreateGuildEmoji(ulong guildId, CreateEmojiParams body)
        => new (nameof(CreateGuildEmoji),
            RequestMethod.Post,
            $"guilds/{guildId}/emojis",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyEmojiParams, API.Emoji> ModifyGuildEmoji(ulong guildId, ulong emojiId, ModifyEmojiParams body)
        => new(nameof(ModifyGuildEmoji),
            RequestMethod.Patch,
            $"guilds/{guildId}/emojis/{emojiId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteGuildEmoji(ulong guildId, ulong emojiId)
        => new(nameof(DeleteGuildEmoji),
            RequestMethod.Delete,
            $"guilds/{guildId}/emojis/{emojiId}",
            (ScopeType.Guild, guildId));
}

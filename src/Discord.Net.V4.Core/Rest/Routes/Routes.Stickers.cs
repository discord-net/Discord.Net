using Discord.API;
using Discord.Rest;

namespace Discord;

public static partial class Routes
{
    public static APIRoute<Sticker> GetSticker(ulong stickerId)
        => new(nameof(GetSticker), RequestMethod.Get, $"stickers/{stickerId}");

    public static readonly APIRoute<StickerPack[]> ListStickerPacks
        = new(nameof(ListStickerPacks), RequestMethod.Get, "sticker-packs");

    public static APIRoute<Sticker[]> ListGuildStickers(ulong guildId)
        => new(nameof(ListGuildStickers), RequestMethod.Get, $"guilds/{guildId}/stickers", (ScopeType.Guild, guildId));

    public static APIRoute<Sticker> GetGuildSticker(ulong guildId, ulong stickerId)
        => new(nameof(GetGuildSticker), RequestMethod.Get, $"guilds/{guildId}/stickers/{stickerId}", (ScopeType.Guild, guildId));

    public static APIBodyRoute<CreateStickerProperties, Sticker> CreateGuildSticker(ulong guildId, CreateStickerProperties body)
        => new(nameof(CreateGuildSticker), RequestMethod.Post, $"guilds/{guildId}/stickers", body, ContentType.MultipartForm, (ScopeType.Guild, guildId));

    public static APIBodyRoute<ModifyStickerProperties, Sticker> ModifyGuildSticker(ulong guildId, ModifyStickerProperties body)
        => new(nameof(ModifyGuildSticker), RequestMethod.Patch, $"guilds/{guildId}/stickers", body, bucket: (ScopeType.Guild, guildId));

    public static APIRoute DeleteGuildSticker(ulong guildId, ulong stickerId)
        => new(nameof(DeleteGuildSticker), RequestMethod.Delete, $"guilds/{guildId}/stickers/{stickerId}", (ScopeType.Guild, guildId));
}

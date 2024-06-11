using Discord.Models.Json;
using Discord.Models.Json.Stickers;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly ApiRoute<StickerPack[]> ListStickerPacks
        = new(nameof(ListStickerPacks),
            RequestMethod.Get,
            "sticker-packs");

    public static ApiRoute<Sticker> GetSticker(ulong stickerId)
        => new(nameof(GetSticker),
            RequestMethod.Get,
            $"stickers/{stickerId}");

    public static ApiRoute<Sticker[]> ListGuildStickers(ulong guildId)
        => new(nameof(ListGuildStickers),
            RequestMethod.Get,
            $"guilds/{guildId}/stickers",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Sticker> GetGuildSticker(ulong guildId, ulong stickerId)
        => new(nameof(GetGuildSticker),
            RequestMethod.Get,
            $"guilds/{guildId}/stickers/{stickerId}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateStickerProperties, Sticker> CreateGuildSticker(ulong guildId,
        CreateStickerProperties body)
        => new(nameof(CreateGuildSticker),
            RequestMethod.Post,
            $"guilds/{guildId}/stickers",
            body,
            ContentType.MultipartForm,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildStickersParams, Sticker> ModifyGuildSticker(ulong guildId, ulong stickerId,
        ModifyGuildStickersParams body)
        => new(nameof(ModifyGuildSticker),
            RequestMethod.Patch,
            $"guilds/{guildId}/stickers/{stickerId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteGuildSticker(ulong guildId, ulong stickerId)
        => new(nameof(DeleteGuildSticker),
            RequestMethod.Delete,
            $"guilds/{guildId}/stickers/{stickerId}",
            (ScopeType.Guild, guildId));
}

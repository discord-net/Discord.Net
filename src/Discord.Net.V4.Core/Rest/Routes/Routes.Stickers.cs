using Discord.Models.Json;
using Discord.Models.Json.Stickers;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly IApiOutRoute<StickerPack[]> ListStickerPacks
        = new ApiOutRoute<StickerPack[]>(nameof(ListStickerPacks), RequestMethod.Get, "sticker-packs");

    public static IApiOutRoute<Sticker> GetSticker(ulong id) =>
        new ApiOutRoute<Sticker>(nameof(GetSticker), RequestMethod.Get, $"stickers/{id}");

    public static IApiOutRoute<Sticker[]> ListGuildStickers(ulong guildId) =>
        new ApiOutRoute<Sticker[]>(nameof(ListGuildStickers), RequestMethod.Get, $"guilds/{guildId}/stickers",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Sticker> GetGuildSticker(ulong guildId, ulong id) =>
        new ApiOutRoute<Sticker>(nameof(GetGuildSticker), RequestMethod.Get, $"guilds/{guildId}/stickers/{id}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateStickerProperties, Sticker> CreateGuildSticker(ulong guildId,
        CreateStickerProperties body) =>
        new ApiInOutRoute<CreateStickerProperties, Sticker>(nameof(CreateGuildSticker), RequestMethod.Post,
            $"guilds/{guildId}/stickers", body, ContentType.MultipartForm, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildStickersParams, Sticker> ModifyGuildSticker(ulong guildId, ulong stickerId,
        ModifyGuildStickersParams body) =>
        new ApiInOutRoute<ModifyGuildStickersParams, Sticker>(nameof(ModifyGuildSticker), RequestMethod.Patch,
            $"guilds/{guildId}/stickers/{stickerId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildSticker(ulong guildId, ulong stickerId) =>
        new ApiRoute(nameof(DeleteGuildSticker), RequestMethod.Delete, $"guilds/{guildId}/stickers/{stickerId}",
            (ScopeType.Guild, guildId));
}

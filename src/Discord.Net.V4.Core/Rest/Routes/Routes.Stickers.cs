using Discord.Models.Json;
using Discord.Models.Json.Stickers;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly IApiOutRoute<StickerPack[]> ListStickerPacks
        = new ApiOutRoute<StickerPack[]>(nameof(ListStickerPacks), RequestMethod.Get, "sticker-packs");

    public static IApiOutRoute<Sticker> GetSticker([IdHeuristic<ISticker>] ulong stickerId) =>
        new ApiOutRoute<Sticker>(nameof(GetSticker), RequestMethod.Get, $"stickers/{stickerId}");

    public static IApiOutRoute<Sticker[]> ListGuildStickers([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<Sticker[]>(nameof(ListGuildStickers), RequestMethod.Get, $"guilds/{guildId}/stickers",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Sticker> GetGuildSticker([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildSticker>] ulong stickerId) =>
        new ApiOutRoute<Sticker>(nameof(GetGuildSticker), RequestMethod.Get, $"guilds/{guildId}/stickers/{stickerId}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateStickerProperties, Sticker> CreateGuildSticker(
        [IdHeuristic<IGuild>] ulong guildId,
        CreateStickerProperties body) =>
        new ApiInOutRoute<CreateStickerProperties, Sticker>(nameof(CreateGuildSticker), RequestMethod.Post,
            $"guilds/{guildId}/stickers", body, ContentType.MultipartForm, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildStickersParams, Sticker> ModifyGuildSticker(
        [IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IGuildSticker>] ulong stickerId,
        ModifyGuildStickersParams body) =>
        new ApiInOutRoute<ModifyGuildStickersParams, Sticker>(nameof(ModifyGuildSticker), RequestMethod.Patch,
            $"guilds/{guildId}/stickers/{stickerId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildSticker([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildSticker>] ulong stickerId) =>
        new ApiRoute(nameof(DeleteGuildSticker), RequestMethod.Delete, $"guilds/{guildId}/stickers/{stickerId}",
            (ScopeType.Guild, guildId));
}

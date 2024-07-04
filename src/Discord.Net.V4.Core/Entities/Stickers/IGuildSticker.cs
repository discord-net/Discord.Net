using Discord.Models;
using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IGuildSticker, ModifyStickerProperties, ModifyGuildStickersParams, IStickerModel>;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
public interface IGuildSticker :
    ISticker,
    IGuildStickerActor,
    IRefreshable<IGuildSticker, ulong, IStickerModel>,
    IModifiable
{
    static IApiInOutRoute<ModifyGuildStickersParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildStickersParams args
    ) => Routes.ModifyGuildSticker(path.Require<IGuild>(), id, args);

    static IApiOutRoute<IStickerModel> IRefreshable<IGuildSticker, ulong, IStickerModel>.RefreshRoute(
        IGuildSticker self,
        ulong id) => Routes.GetGuildSticker(self.Require<IGuild>(), id);

    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    ILoadableGuildMemberActor? Author { get; }

    new IStickerModel GetModel();
}

using Discord.Models;
using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IGuildSticker, ModifyStickerProperties, ModifyGuildStickersParams, IGuildStickerModel>;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
public interface IGuildSticker :
    ISticker,
    IGuildStickerActor,
    IRefreshable<IGuildSticker, ulong, IGuildStickerModel>,
    IModifiable
{
    static IApiInOutRoute<ModifyGuildStickersParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildStickersParams args
    ) => Routes.ModifyGuildSticker(path.Require<IGuild>(), id, args);

    static IApiOutRoute<IGuildStickerModel> IRefreshable<IGuildSticker, ulong, IGuildStickerModel>.RefreshRoute(
        IGuildSticker self,
        ulong id) => Routes.GetGuildSticker(self.Require<IGuild>(), id);

    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    ILoadableGuildMemberActor? Author { get; }

    /// <summary>
    ///     Gets whether this guild sticker can be used, may be false due to loss of Server Boosts.
    /// </summary>
    bool? IsAvailable { get; }

    new IGuildStickerModel GetModel();

    new Task RefreshAsync(RequestOptions? options = null, CancellationToken token = default)
        => ((IRefreshable<IGuildSticker, ulong, IGuildStickerModel>)this).RefreshAsync(options, token);
}

using Discord.Models;
using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IGuildSticker, ModifyStickerProperties, ModifyGuildStickersParams, IGuildStickerModel>;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
[Refreshable(nameof(Routes.GetGuildSticker))]
public partial interface IGuildSticker :
    ISticker,
    IGuildStickerActor,
    IEntityOf<IGuildStickerModel>
{
    [SourceOfTruth]
    new IGuildStickerModel GetModel();

    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    ILoadableGuildMemberActor? Author { get; }

    /// <summary>
    ///     Gets whether this guild sticker can be used, may be false due to loss of Server Boosts.
    /// </summary>
    bool? IsAvailable { get; }
}

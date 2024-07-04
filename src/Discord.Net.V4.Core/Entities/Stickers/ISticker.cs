using Discord.Models;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a discord sticker.
/// </summary>
public interface ISticker : ISnowflakeEntity, IRefreshable<ISticker, ulong, IStickerModel>
{
    static IApiOutRoute<IStickerModel> IRefreshable<ISticker, ulong, IStickerModel>.RefreshRoute(
        ISticker self,
        ulong id
    ) => Routes.GetSticker(id);

    /// <summary>
    ///     The name of the sticker.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The format of the sticker.
    /// </summary>
    StickerFormatType Format { get; }

    /// <summary>
    ///     Gets the ID of the pack of this sticker.
    /// </summary>
    /// <returns>
    ///     A snowflake ID associated with the pack of this sticker.
    /// </returns>
    ulong? PackId { get; }

    /// <summary>
    ///     Gets the description of this sticker.
    /// </summary>
    /// <returns>
    ///     A <see langword="string" /> with the description of this sticker.
    /// </returns>
    string? Description { get; }

    /// <summary>
    ///     Gets the list of tags of this sticker.
    /// </summary>
    /// <returns>
    ///     A read-only list with the tags of this sticker.
    /// </returns>
    IReadOnlyCollection<string> Tags { get; }

    /// <summary>
    ///     Gets the type of this sticker.
    /// </summary>
    StickerType Type { get; }

    /// <summary>
    ///     Gets whether this guild sticker can be used, may be false due to loss of Server Boosts.
    /// </summary>
    bool? IsAvailable { get; }

    /// <summary>
    ///     Gets the standard sticker's sort order within its pack.
    /// </summary>
    int? SortOrder { get; }
}

namespace Discord;

/// <summary>
///     Represents a discord sticker.
/// </summary>
public interface ISticker : IStickerItem
{
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
    ///     A <see langword="string"/> with the description of this sticker.
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

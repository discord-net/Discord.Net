using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a discord sticker.
    /// </summary>
    public interface ISticker
    {
        /// <summary>
        ///     Gets the ID of this sticker.
        /// </summary>
        /// <returns>
        ///     A snowflake ID associated with this sticker.
        /// </returns>
        ulong Id { get; }
        /// <summary>
        ///     Gets the ID of the pack of this sticker.
        /// </summary>
        /// <returns>
        ///     A snowflake ID associated with the pack of this sticker.
        /// </returns>
        ulong PackId { get; }
        /// <summary>
        ///     Gets the name of this sticker.
        /// </summary>
        /// <returns>
        ///     A <see langword="string"/> with the name of this sticker.
        /// </returns>
        string Name { get; }
        /// <summary>
        ///     Gets the description of this sticker.
        /// </summary>
        /// <returns>
        ///     A <see langword="string"/> with the description of this sticker.
        /// </returns>
        string Description { get; }
        /// <summary>
        ///     Gets the list of tags of this sticker.
        /// </summary>
        /// <returns>
        ///     A read-only list with the tags of this sticker.
        /// </returns>
        IReadOnlyCollection<string> Tags { get; }
        /// <summary>
        ///     Gets the asset hash of this sticker.
        /// </summary>
        /// <returns>
        ///     A <see langword="string"/> with the asset hash of this sticker.
        /// </returns>
        string Asset { get; }
        /// <summary>
        ///     Gets the preview asset hash of this sticker.
        /// </summary>
        /// <returns>
        ///     A <see langword="string"/> with the preview asset hash of this sticker.
        /// </returns>
        string PreviewAsset { get; }
        /// <summary>
        ///     Gets the format type of this sticker.
        /// </summary>
        /// <returns>
        ///     A <see cref="StickerFormatType"/> with the format type of this sticker.
        /// </returns>
        StickerFormatType FormatType { get; }
    }
}

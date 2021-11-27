namespace Discord
{
    /// <summary>
    ///     Represents a partial sticker item received with a message.
    /// </summary>
    public interface IStickerItem
    {
        /// <summary>
        ///     The id of the sticker.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        ///     The name of the sticker.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The format of the sticker.
        /// </summary>
        StickerFormatType Format { get; }
    }
}

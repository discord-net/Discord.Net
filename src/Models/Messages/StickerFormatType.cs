namespace Discord.Net.Models
{
    /// <summary>
    /// Declares a flag enum which represents the format type for a <see cref="Sticker"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-sticker-format-types"/>
    /// </remarks>
    public enum StickerFormatType
    {
        /// <summary>
        /// The sticker format is a png.
        /// </summary>
        Png = 1,

        /// <summary>
        /// The sticker format is a apng.
        /// </summary>
        Apng = 2,

        /// <summary>
        /// The sticker format is a lottie.
        /// </summary>
        Lottie = 3,
    }
}

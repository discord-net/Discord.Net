using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the sticker format type.
    /// </summary>
    [Flags]
    public enum StickerFormatType
    {
        /// <summary>
        ///     The sticker format is a png.
        /// </summary>
        Png = 1,

        /// <summary>
        ///     The sticker format is a apng.
        /// </summary>
        Apng = 2,

        /// <summary>
        ///     The sticker format is a lottie.
        /// </summary>
        Lottie = 3,
    }
}

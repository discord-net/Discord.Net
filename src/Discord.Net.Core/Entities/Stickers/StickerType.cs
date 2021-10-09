namespace Discord
{
    /// <summary>
    ///     Represents a type of sticker..
    /// </summary>
    public enum StickerType
    {
        /// <summary>
        ///     Represents a discord standard sticker, this type of sticker cannot be modified by an application.
        /// </summary>
        Standard = 1,

        /// <summary>
        ///     Represents a sticker that was created within a guild.
        /// </summary>
        Guild = 2
    }
}

namespace Discord
{
    /// <summary>
    ///     Specifies the type of embed.
    /// </summary>
    public enum EmbedType
    {
        /// <summary>
        ///     An unknown embed type.
        /// </summary>
        Unknown = -1,
        /// <summary>
        ///     A rich embed type.
        /// </summary>
        Rich,
        /// <summary>
        ///     A link embed type.
        /// </summary>
        Link,
        /// <summary>
        ///     A video embed type.
        /// </summary>
        Video,
        /// <summary>
        ///     An image embed type.
        /// </summary>
        Image,
        /// <summary>
        ///     A GIFV embed type.
        /// </summary>
        Gifv,
        /// <summary>
        ///     An article embed type.
        /// </summary>
        Article,
        /// <summary>
        ///     A tweet embed type.
        /// </summary>
        Tweet,
        /// <summary>
        ///     A HTML embed type.
        /// </summary>
        Html,
    }
}

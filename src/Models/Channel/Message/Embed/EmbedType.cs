namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the type of embed.
    /// </summary>
    public enum EmbedType
    {
        /// <summary>
        ///     Generic embed rendered from embed attributes.
        /// </summary>
        Rich,

        /// <summary>
        ///     Image embed.
        /// </summary>
        Image,

        /// <summary>
        ///     Video embed.
        /// </summary>
        Video,

        /// <summary>
        ///     Animated gif image embed rendered as a video embed.
        /// </summary>
        Gifv,

        /// <summary>
        ///     Article embed.
        /// </summary>
        Article,

        /// <summary>
        ///     Link embed.
        /// </summary>
        Link,
    }
}

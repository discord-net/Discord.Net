namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the type of embed for an <see cref="Embed"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-types"/>
    /// </remarks>
    public enum EmbedType
    {
        /// <summary>
        /// Generic embed rendered from embed attributes.
        /// </summary>
        Rich,

        /// <summary>
        /// Image embed.
        /// </summary>
        Image,

        /// <summary>
        /// Video embed.
        /// </summary>
        Video,

        /// <summary>
        /// Animated gif image embed rendered as a video embed.
        /// </summary>
        Gifv,

        /// <summary>
        /// Article embed.
        /// </summary>
        Article,

        /// <summary>
        /// Link embed.
        /// </summary>
        Link,
    }
}

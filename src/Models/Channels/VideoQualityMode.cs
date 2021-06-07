namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the video quality mode.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#channel-object-video-quality-modes"/>
    /// </remarks>
    public enum VideoQualityMode
    {
        /// <summary>
        /// Discord chooses the quality for optimal performance.
        /// </summary>
        Auto = 1,

        /// <summary>
        /// 720p.
        /// </summary>
        Full = 2,
    }
}

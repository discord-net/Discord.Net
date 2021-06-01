using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the video quality mode type.
    /// </summary>
    [Flags]
    public enum VideoQualityMode
    {
        /// <summary>
        ///     Discord chooses the quality for optimal performance.
        /// </summary>
        Auto = 1,

        /// <summary>
        ///     720p.
        /// </summary>
        Full = 2,
    }
}

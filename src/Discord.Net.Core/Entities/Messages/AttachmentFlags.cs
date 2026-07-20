using System;

namespace Discord;

[Flags]
public enum AttachmentFlags
{
    /// <summary>
    ///     The attachment has no flags.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Indicates that this attachment is a clip.
    /// </summary>
    IsClip = 1 << 0,

    /// <summary>
    ///     Indicates that this attachment is a thumbnail.
    /// </summary>
    IsThumbnail = 1 << 1,
    
    /// <summary>
    ///     Indicates that this attachment has been edited using the remix feature on mobile.
    /// </summary>
    [Obsolete("Discord's remix feature is deprecated.")]
    IsRemix = 1 << 2,

    /// <summary>
    ///     Hides the attachment behind a spoiler warning.
    /// </summary>
    IsSpoiler = 1 << 3,

    /// <summary>
    ///     Indicates that the attachment contains animated content.
    /// </summary>
    IsAnimated = 1 << 5,
}

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
    IsRemix = 1 << 2,
}

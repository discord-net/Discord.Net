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
    ///     Indicates that this attachment has been edited using the remix feature on mobile.
    /// </summary>
    Remix = 0,
}

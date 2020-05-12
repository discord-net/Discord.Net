using System;

namespace Discord
{
    /// <summary>
    ///     Specifies the type of mentions that will be notified from the message content.
    /// </summary>
    [Flags]
    public enum AllowedMentionTypes
    {
        /// <summary>
        ///     No flag was set.
        /// </summary>
        /// <remarks>
        ///     It is not used to control mentions and does not mean no mentions will be allowed.
        /// </remarks>
        None        = 0,
        /// <summary>
        ///     Controls role mentions.
        /// </summary>
        Roles       = 1,
        /// <summary>
        ///     Controls user mentions.
        /// </summary>
        Users       = 2,
        /// <summary>
        ///     Controls <code>@everyone</code> and <code>@here</code> mentions.
        /// </summary>
        Everyone    = 4,
    }
}

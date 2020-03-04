using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Defines which types of mentions will be parsed from a created message's content.
    /// </summary>
    public class AllowedMentions
    {
        /// <summary>
        ///     Gets or sets the allowed mention types to parse from the message content.
        ///     If <c>null</c>, no users will be notified.
        /// </summary>
        public AllowedMentionTypes? AllowedTypes { get; set; }

        /// <summary>
        ///     Gets or sets the list of all role Ids that can be mentioned.
        /// </summary>
        public List<ulong> RoleIds { get; set; }

        /// <summary>
        ///     Gets or sets the list of all user Ids that can be mentioned.
        /// </summary>
        public List<ulong> UserIds { get; set; }
    }
}

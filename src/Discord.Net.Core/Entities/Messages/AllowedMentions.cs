using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Defines which mentions and types of mentions that will notify users from the message text.
    /// </summary>
    public class AllowedMentions
    {
        /// <summary>
        ///     Gets or sets the type of mentions that will notify users.
        ///     If <c>null</c>, only the role and user Ids specified in <see cref="RoleIds"/>
        ///     and <see cref="UserIds"/> respectively will be notified.
        /// </summary>
        public AllowedMentionTypes? AllowedTypes { get; set; }

        /// <summary>
        ///     Gets or sets the list of all role Ids that will be mentioned.
        /// </summary>
        public List<ulong> RoleIds { get; set; }

        /// <summary>
        ///     Gets or sets the list of all user Ids that will be mentioned.
        /// </summary>
        public List<ulong> UserIds { get; set; }
    }
}

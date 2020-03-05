using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Defines which mentions and types of mentions that will notify users from the message content.
    /// </summary>
    public class AllowedMentions
    {
        /// <summary>
        ///     Gets or sets the type of mentions that will be parsed from the message content.
        ///     The <see cref="AllowedMentionTypes.Users"/> flag is mutually exclusive with the <see cref="UserIds"/>
        ///     property, and the <see cref="AllowedMentionTypes.Roles"/> flag is mutually exclusive with the
        ///     <see cref="RoleIds"/> property.
        /// </summary>
        public AllowedMentionTypes? AllowedTypes { get; set; }

        /// <summary>
        ///     Gets or sets the list of all role Ids that will be mentioned.
        ///     This property is mutually exclusive with the <see cref="AllowedMentionTypes.Roles"/>
        ///     flag of the <see cref="AllowedTypes"/> property. If the flag is set, the value of this property
        ///     must be <c>null</c> or empty.
        /// </summary>
        public List<ulong> RoleIds { get; set; }

        /// <summary>
        ///     Gets or sets the list of all user Ids that will be mentioned.
        ///     This property is mutually exclusive with the <see cref="AllowedMentionTypes.Users"/>
        ///     flag of the <see cref="AllowedTypes"/> property. If the flag is set, the value of this property
        ///     must be <c>null</c> or empty.
        /// </summary>
        public List<ulong> UserIds { get; set; }
    }
}

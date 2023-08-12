using System;
using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Defines which mentions and types of mentions that will notify users from the message content.
    /// </summary>
    public class AllowedMentions
    {
        private static readonly Lazy<AllowedMentions> none = new Lazy<AllowedMentions>(() => new AllowedMentions());
        private static readonly Lazy<AllowedMentions> all = new Lazy<AllowedMentions>(() =>
            new AllowedMentions(AllowedMentionTypes.Everyone | AllowedMentionTypes.Users | AllowedMentionTypes.Roles));

        /// <summary>
        ///     Gets a value which indicates that no mentions in the message content should notify users.
        /// </summary>
        public static AllowedMentions None => none.Value;

        /// <summary>
        ///     Gets a value which indicates that all mentions in the message content should notify users.
        /// </summary>
        public static AllowedMentions All => all.Value;

        /// <summary>
        ///     Gets or sets the type of mentions that will be parsed from the message content.
        /// </summary>
        /// <remarks>
        ///     The <see cref="AllowedMentionTypes.Users"/> flag is mutually exclusive with the <see cref="UserIds"/>
        ///     property, and the <see cref="AllowedMentionTypes.Roles"/> flag is mutually exclusive with the
        ///     <see cref="RoleIds"/> property.
        ///     If <see langword="null" />, only the ids specified in <see cref="UserIds"/> and <see cref="RoleIds"/> will be mentioned.
        /// </remarks>
        public AllowedMentionTypes? AllowedTypes { get; set; }

        /// <summary>
        ///     Gets or sets the list of all role ids that will be mentioned.
        ///     This property is mutually exclusive with the <see cref="AllowedMentionTypes.Roles"/>
        ///     flag of the <see cref="AllowedTypes"/> property. If the flag is set, the value of this property
        ///     must be <see langword="null" /> or empty.
        /// </summary>
        public List<ulong> RoleIds { get; set; } = new List<ulong>();

        /// <summary>
        ///     Gets or sets the list of all user ids that will be mentioned.
        ///     This property is mutually exclusive with the <see cref="AllowedMentionTypes.Users"/>
        ///     flag of the <see cref="AllowedTypes"/> property. If the flag is set, the value of this property
        ///     must be <see langword="null" /> or empty.
        /// </summary>
        public List<ulong> UserIds { get; set; } = new List<ulong>();

        /// <summary>
        ///     Gets or sets whether to mention the author of the message you are replying to or not.
        /// </summary>
        /// <remarks>
        ///     Specifically for inline replies.
        /// </remarks>
        public bool? MentionRepliedUser { get; set; } = null;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AllowedMentions"/> class.
        /// </summary>
        /// <param name="allowedTypes">
        ///     The types of mentions to parse from the message content.
        ///     If <see langword="null" />, only the ids specified in <see cref="UserIds"/> and <see cref="RoleIds"/> will be mentioned.
        /// </param>
        public AllowedMentions(AllowedMentionTypes? allowedTypes = null)
        {
            AllowedTypes = allowedTypes;
        }
    }
}

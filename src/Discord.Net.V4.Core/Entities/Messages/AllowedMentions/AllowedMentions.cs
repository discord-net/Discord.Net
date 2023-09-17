using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Defines which mentions and types of mentions that will notify users from the message content.
/// </summary>
public readonly struct AllowedMentions
{
    public static readonly AllowedMentions None = new();
    public static readonly AllowedMentions All = new(AllowedMentionTypes.Everyone | AllowedMentionTypes.Users | AllowedMentionTypes.Roles);

    /// <summary>
    ///     The type of mentions that will be parsed from the message content.
    /// </summary>
    /// <remarks>
    ///     The <see cref="AllowedMentionTypes.Users"/> flag is mutually exclusive with the <see cref="UserIds"/>
    ///     property, and the <see cref="AllowedMentionTypes.Roles"/> flag is mutually exclusive with the
    ///     <see cref="RoleIds"/> property.
    ///     If <see cref="AllowedMentionTypes.None"/>, only the ids specified in <see cref="UserIds"/> and <see cref="RoleIds"/> will be mentioned.
    /// </remarks>
    public readonly AllowedMentionTypes AllowedTypes;

    /// <summary>
    ///     The collection of all role ids that will be mentioned.
    ///     This property is mutually exclusive with the <see cref="AllowedMentionTypes.Roles"/>
    ///     flag of the <see cref="AllowedTypes"/> property. If the flag is set, the value of this property
    ///     must be empty.
    /// </summary>
    public readonly ImmutableArray<ulong> RoleIds;

    /// <summary>
    ///     The collection of all user ids that will be mentioned.
    ///     This property is mutually exclusive with the <see cref="AllowedMentionTypes.Users"/>
    ///     flag of the <see cref="AllowedTypes"/> property. If the flag is set, the value of this property
    ///     must be <see langword="null" /> or empty.
    /// </summary>
    public readonly ImmutableArray<ulong> UserIds;

    /// <summary>
    ///     Whether to mention the author of the message you are replying to or not.
    /// </summary>
    /// <remarks>
    ///     Specifically for inline replies.
    /// </remarks>
    public readonly bool MentionRepliedUser;

    public AllowedMentions(AllowedMentionTypes allowedTypes)
        : this(allowedTypes, Array.Empty<ulong>(), Array.Empty<ulong>(), false)
    { }

    public AllowedMentions(
        AllowedMentionTypes allowedTypes, IEnumerable<ulong> roleIds,
        IEnumerable<ulong> userIds, bool mentionRepliedUser)
    {
        AllowedTypes = allowedTypes;
        RoleIds = roleIds.ToImmutableArray();
        UserIds = userIds.ToImmutableArray();
        MentionRepliedUser = mentionRepliedUser;
    }
}

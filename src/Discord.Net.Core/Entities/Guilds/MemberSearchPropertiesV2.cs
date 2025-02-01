using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;


/// <summary>
///     Represents the properties for searching members in a guild.
/// </summary>
public class MemberSearchPropertiesV2
{
    /// <summary>
    ///     Gets or sets the after property for the search.
    /// </summary>
    public MemberSearchPaginationFilter? After { get; set; }

    /// <summary>
    ///     Gets or sets the before property for the search.
    /// </summary>
    public MemberSearchPaginationFilter? Before { get; set; }

    /// <summary>
    ///     Gets or sets the sort type for the search.
    /// </summary>
    public MemberSearchV2SortType? Sort { get; set; }

    /// <summary>
    ///     Gets or sets the and query for the search.
    /// </summary>
    public MemberSearchFilter? AndQuery { get; set; }

    /// <summary>
    ///     Gets or sets the or query for the search.
    /// </summary>
    public MemberSearchFilter? OrQuery { get; set; }
}


/// <summary>
///     Represents the after property for searching members in a guild.
/// </summary>
public struct MemberSearchPaginationFilter
{
    /// <summary>
    ///     Gets or sets the user ID to search after.
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    ///     Gets or sets the guild joined at timestamp to search after.
    /// </summary>
    public long GuildJoinedAt { get; set; }

    public MemberSearchPaginationFilter(ulong userId, long guildJoinedAt)
    {
        UserId = userId;
        GuildJoinedAt = guildJoinedAt;
    }

    public MemberSearchPaginationFilter(ulong userId, DateTimeOffset guildJoinedAt)
    {
        UserId = userId;
        GuildJoinedAt = guildJoinedAt.ToUnixTimeMilliseconds();
    }

    public MemberSearchPaginationFilter() { }
}


/// <summary>
///     Represents the query parameters for searching members in a guild.
/// </summary>
public struct MemberSearchFilter
{
    /// <summary>
    ///     Gets or sets the safety signal search properties.
    /// </summary>
    public MemberSearchV2SafetySignalsProperties? SafetySignals { get; set; }

    /// <summary>
    ///     Gets or sets the role IDs to search for.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="IMemberSearchQuery.OrQuery"/> and <see cref="IMemberSearchQuery.AndQuery"/> are supported.
    /// </remarks>
    public MemberSearchSnowflakeQuery? RoleIds { get; set; }

    /// <summary>
    ///     Gets or sets the range to search for the user ID.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="IMemberSearchQuery.OrQuery"/> and <see cref="IMemberSearchQuery.Range"/> are supported.
    /// </remarks>
    public MemberSearchSnowflakeQuery? UserId { get; set; }

    /// <summary>
    ///     Gets or sets the range to search for the user's guild joined at timestamp.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="IMemberSearchQuery.Range"/> is supported.
    /// </remarks>
    public MemberSearchIntQuery? GuildJoinedAt { get; set; }

    /// <summary>
    ///     Gets or sets the source invite code to search for.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="IMemberSearchQuery.OrQuery"/> is supported.
    /// </remarks>
    public MemberSearchStringQuery? SourceInviteCode { get; set; }

    /// <summary>
    ///     Gets or sets the join source type to search for.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="IMemberSearchQuery.OrQuery"/> is supported.
    /// </remarks>
    public MemberSearchIntQuery? JoinSourceType { get; set; }

    /// <summary>
    ///     Gets or sets whether the member left and rejoined the guild.
    /// </summary>
    public bool? DidRejoin { get; set; }

    /// <summary>
    ///     Gets or sets whether the member has not yet passed the guild's member verification requirements.
    /// </summary>
    public bool? IsPending { get; set; }

    /// <summary>
    ///     Gets or sets the usernames to match against.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="IMemberSearchQuery.OrQuery"/> is supported.
    /// </remarks>
    public MemberSearchStringQuery? Usernames { get; set; }
}


/// <summary>
///     Represents the safety signal properties for searching members in a guild.
/// </summary>
public struct MemberSearchV2SafetySignalsProperties
{
    /// <summary>
    ///     Gets or sets the unusual DM activity until property for the search.
    /// </summary>
    public MemberSearchIntQuery? UnusualDmActivityUntil { get; set; }

    /// <summary>
    ///     Gets or sets the communication disabled until property for the search.
    /// </summary>
    public MemberSearchIntQuery? CommunicationDisabledUntil { get; set; }

    /// <summary>
    ///     Gets or sets the unusual account activity property for the search.
    /// </summary>
    public bool? UnusualAccountActivity { get; set; }

    /// <summary>
    ///     Gets or sets the automod quarantined username property for the search.
    /// </summary>
    public bool? AutomodQuarantinedUsername { get; set; }
}

/// <summary>
///     Represents the range properties for searching members in a guild.
/// </summary>
public struct MemberSearchV2Range
{
    /// <summary>
    ///     Gets or sets the less than property for the search.
    /// </summary>
    public long? LessThanOrEqual { get; set; }

    /// <summary>
    ///     Gets or sets the greater than property for the search.
    /// </summary>
    public long? GreaterThanOrEqual { get; set; }
}


public interface IMemberSearchQuery
{
    /// <summary>
    ///     Gets or sets the range for the search.
    /// </summary>
    MemberSearchV2Range? Range { get; }

    /// <summary>
    ///     Gets the AND query for the search.
    /// </summary>
    IEnumerable<object> AndQuery { get; }

    /// <summary>
    ///     Gets the OR query for the search.
    /// </summary>
    IEnumerable<object> OrQuery { get; }
}

public struct MemberSearchStringQuery : IMemberSearchQuery
{
    /// <inheritdoc />
    public MemberSearchV2Range? Range { get; set; }

    /// <inheritdoc cref="IMemberSearchQuery.AndQuery"/>
    public IEnumerable<string> AndQuery { get; set; }

    /// <inheritdoc cref="IMemberSearchQuery.OrQuery"/>
    public IEnumerable<string> OrQuery { get; set; }

    /// <inheritdoc />
    IEnumerable<object> IMemberSearchQuery.AndQuery => AndQuery;

    /// <inheritdoc />
    IEnumerable<object> IMemberSearchQuery.OrQuery => OrQuery;
}


public struct MemberSearchIntQuery : IMemberSearchQuery
{
    /// <inheritdoc />
    public MemberSearchV2Range? Range { get; set; }

    /// <inheritdoc cref="IMemberSearchQuery.AndQuery"/>
    public IEnumerable<int> AndQuery { get; set; }

    /// <inheritdoc cref="IMemberSearchQuery.OrQuery"/>
    public IEnumerable<int> OrQuery { get; set; }

    /// <inheritdoc />
    IEnumerable<object> IMemberSearchQuery.AndQuery => AndQuery?.Select(x => (object)x);

    /// <inheritdoc />
    IEnumerable<object> IMemberSearchQuery.OrQuery => OrQuery?.Select(x => (object)x);
}


public struct MemberSearchSnowflakeQuery : IMemberSearchQuery
{
    /// <inheritdoc />
    public MemberSearchV2Range? Range { get; set; }

    /// <inheritdoc cref="IMemberSearchQuery.AndQuery"/>
    public IEnumerable<ulong> AndQuery { get; set; }

    /// <inheritdoc cref="IMemberSearchQuery.OrQuery"/>
    public IEnumerable<ulong> OrQuery { get; set; }

    /// <inheritdoc />
    IEnumerable<object> IMemberSearchQuery.AndQuery => AndQuery?.Select(x => (object)x);

    /// <inheritdoc />
    IEnumerable<object> IMemberSearchQuery.OrQuery => OrQuery?.Select(x => (object)x);
}

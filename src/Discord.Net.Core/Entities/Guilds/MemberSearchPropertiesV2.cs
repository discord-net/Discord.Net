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
    public MemberSearchPropertiesV2After After { get; set; }

    /// <summary>
    ///     Gets or sets the sort type for the search.
    /// </summary>
    public MemberSearchV2SortType? Sort { get; set; }

    /// <summary>
    ///     Gets or sets the and query for the search.
    /// </summary>
    public MemberSearchV2QueryParams? AndQuery { get; set; }

    /// <summary>
    ///     Gets or sets the or query for the search.
    /// </summary>
    public MemberSearchV2QueryParams? OrQuery { get; set; }
}


/// <summary>
///     Represents the after property for searching members in a guild.
/// </summary>
public struct MemberSearchPropertiesV2After
{
    /// <summary>
    ///     Gets or sets the user ID to search after.
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    ///     Gets or sets the guild joined at timestamp to search after.
    /// </summary>
    public long GuildJoinedAt { get; set; }

    public MemberSearchPropertiesV2After(ulong userId, long guildJoinedAt)
    {
        UserId = userId;
        GuildJoinedAt = guildJoinedAt;
    }

    public MemberSearchPropertiesV2After(ulong userId, DateTimeOffset guildJoinedAt)
    {
        UserId = userId;
        GuildJoinedAt = guildJoinedAt.ToUnixTimeMilliseconds();
    }
}


/// <summary>
///     Represents the query parameters for searching members in a guild.
/// </summary>
public struct MemberSearchV2QueryParams
{
    /// <summary>
    ///     Gets or sets the safety signal search properties.
    /// </summary>
    public MemberSearchV2SafetySignalsProperties? SafetySignals { get; set; }

    /// <summary>
    ///     Gets or sets the role IDs to search for.
    /// </summary>
    public MemberSearchV2QueryProperties? RoleIds { get; set; }

    /// <summary>
    ///     Gets or sets the range to search for the user ID.
    /// </summary>
    public MemberSearchV2RangeProperties? UserId { get; set; }

    /// <summary>
    ///     Gets or sets the range to search for the user's guild joined at timestamp.
    /// </summary>
    public MemberSearchV2RangeProperties? GuildJoinedAt { get; set; }

    /// <summary>
    ///     Gets or sets the source invite code to search for.
    /// </summary>
    public MemberSearchV2QueryProperties? SourceInviteCode { get; set; }

    /// <summary>
    ///     Gets or sets the join source type to search for.
    /// </summary>
    public MemberSearchV2QueryProperties? JoinSourceType { get; set; }
}


/// <summary>
///     Represents the safety signal properties for searching members in a guild.
/// </summary>
public struct MemberSearchV2SafetySignalsProperties
{
    /// <summary>
    ///     Gets or sets the unusual DM activity until property for the search.
    /// </summary>
    public MemberSearchV2SafetySignalProperties? UnusualDmActivityUntil { get; set; }

    /// <summary>
    ///     Gets or sets the communication disabled until property for the search.
    /// </summary>
    public MemberSearchV2SafetySignalProperties? CommunicationDisabledUntil { get; set; }

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
///     Represents the query properties for searching members in a guild.
/// </summary>
public readonly struct MemberSearchV2QueryProperties
{
    /// <summary>
    ///     Gets the and query for the search.
    /// </summary>
    public Dictionary<int, object> AndQuery { get; }

    /// <summary>
    ///     Gets the or query for the search.
    /// </summary>
    public Dictionary<int, object> OrQuery { get; }

    public MemberSearchV2QueryProperties(Dictionary<int, string> andQuery, Dictionary<int, string> orQuery)
    {
        AndQuery = andQuery.Select(x => new KeyValuePair<int, object>(x.Key, x.Value)).ToDictionary();
        OrQuery = orQuery.Select(x => new KeyValuePair<int, object>(x.Key, x.Value)).ToDictionary();
    }

    public MemberSearchV2QueryProperties(Dictionary<int, long> andQuery, Dictionary<int, long> orQuery)
    {
        AndQuery = andQuery.Select(x => new KeyValuePair<int, object>(x.Key, x.Value)).ToDictionary();
        OrQuery = orQuery.Select(x => new KeyValuePair<int, object>(x.Key, x.Value)).ToDictionary();
    }
}


/// <summary>
///     Represents the safety signal properties for searching members in a guild.
/// </summary>
public struct MemberSearchV2SafetySignalProperties
{
    /// <summary>
    ///     Gets or sets the range for the search.
    /// </summary>
    public MemberSearchV2RangeProperties Range { get; set; }
}


/// <summary>
///     Represents the range properties for searching members in a guild.
/// </summary>
public struct MemberSearchV2RangeProperties
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

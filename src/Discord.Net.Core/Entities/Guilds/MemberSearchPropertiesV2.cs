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
    public MemberSearchV2SortType Sort { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public MemberSearchV2QueryParams? AndQuery { get; set; }

    /// <summary>
    ///     
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

public struct MemberSearchV2QueryParams
{
    public MemberSearchV2SafetySignalsProperties? SafetySignals { get; set; }

    public MemberSearchV2QueryProperties? RoleIds { get; set; }

    public MemberSearchV2RangeProperties? UserId { get; set; }

    public MemberSearchV2RangeProperties? GuildJoinedAt { get; set; }

    public MemberSearchV2QueryProperties? SourceInviteCode { get; set; }

    public MemberSearchV2QueryProperties? JoinSourceType { get; set; }
}

public struct MemberSearchV2SafetySignalsProperties
{
    public MemberSearchV2SafetySignalProperties? UnusualDmActivityUntil { get; set; }

    public MemberSearchV2SafetySignalProperties? CommunicationDisabledUntil { get; set; }

    public bool? UnusualAccountActivity { get; set; }

    public bool? AutomodQuarantinedUsername { get; set; }
}

public readonly struct MemberSearchV2QueryProperties
{
    public Dictionary<int, object> AndQuery { get; }

    public Dictionary<int, object> OrQuery { get; }

    public MemberSearchV2QueryProperties(Dictionary<int, object> andQuery, Dictionary<int, object> orQuery)
    {
        AndQuery = andQuery;
        OrQuery = orQuery;
    }

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

public struct MemberSearchV2SafetySignalProperties
{
    public MemberSearchV2RangeProperties Range { get; set; }
}

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

using System;

namespace Discord;

public class GuildIncidentsDataProperties
{
    /// <summary>
    ///     Gets or set when invites get enabled again, up to 24 hours in the future.
    /// </summary>
    public Optional<DateTimeOffset?> InvitesDisabledUntil { get; set; }

    /// <summary>
    ///     Gets or set when dms get enabled again, up to 24 hours in the future.
    /// </summary>
    public Optional<DateTimeOffset?> DmsDisabledUntil { get; set; }
}

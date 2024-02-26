using System;

namespace Discord;

public class GuildIncidentsDataProperties
{
    /// <summary>
    ///     Gets or set when invites get enabled again, up to 24 hours in the future.
    /// </summary>
    /// <remarks>
    ///     Set to <see langword="null"/> to enable invites.
    /// </remarks>
    public Optional<DateTimeOffset?> InvitesDisabledUntil { get; set; }

    /// <summary>
    ///     Gets or set when dms get enabled again, up to 24 hours in the future.
    /// </summary>
    /// <remarks>
    ///     Set to <see langword="null"/> to enable dms.
    /// </remarks>
    public Optional<DateTimeOffset?> DmsDisabledUntil { get; set; }
}

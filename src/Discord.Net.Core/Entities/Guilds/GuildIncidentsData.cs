using System;

namespace Discord;

public class GuildIncidentsData
{
    /// <summary>
    ///     Gets the time when invites get enabled again. <see langword="null"/> if invites are not disabled.
    /// </summary>
    public DateTimeOffset? InvitesDisabledUntil { get; set; }

    /// <summary>
    ///     Gets the time when DMs get enabled again. <see langword="null"/> if DMs are not disabled.
    /// </summary>
    public DateTimeOffset? DmsDisabledUntil { get; set; }
}

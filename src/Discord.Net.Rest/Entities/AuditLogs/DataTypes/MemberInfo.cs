using Discord.API.AuditLogs;
using System;

namespace Discord.Rest;

/// <summary>
///     Represents information for a member.
/// </summary>
public struct MemberInfo
{
    internal MemberInfo(MemberInfoAuditLogModel model)
    {
        Nickname = model.Nickname;
        Deaf = model.IsDeafened;
        Mute = model.IsMuted;
        TimedOutUntil = model.TimeOutUntil;
    }

    /// <summary>
    ///     Gets the nickname of the updated member.
    /// </summary>
    /// <returns>
    ///     A string representing the nickname of the updated member; <see langword="null"/> if none is set.
    /// </returns>
    public string Nickname { get; }

    /// <summary>
    ///     Gets a value that indicates whether the updated member is deafened by the guild.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the updated member is deafened (i.e. not permitted to listen to or speak to others) by the guild;
    ///     otherwise <see langword="false"/>.
    ///    <see langword="null"/> if this is not mentioned in this entry.
    /// </returns>
    public bool? Deaf { get; }

    /// <summary>
    ///     Gets a value that indicates whether the updated member is muted (i.e. not permitted to speak via voice) by the
    ///     guild.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the updated member is muted by the guild; otherwise <see langword="false"/>.
    ///     <see langword="null"/> if this is not mentioned in this entry.
    /// </returns>
    public bool? Mute { get; }

    /// <summary>
    ///     Gets the date and time that indicates if and for how long the updated user has been timed out.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> or a timestamp in the past if the user is not timed out.
    /// </remarks>
    /// <returns>
    ///     A <see cref="DateTimeOffset"/> indicating how long the user will be timed out for.
    /// </returns>
    public DateTimeOffset? TimedOutUntil { get; }
}

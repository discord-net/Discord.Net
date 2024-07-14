using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetGuildMember))]
public partial interface IGuildMember :
    IGuildMemberActor,
    ISnowflakeEntity,
    IEntityOf<IMemberModel>
{
    IDefinedLoadableEntityEnumerable<ulong, IRole> Roles { get; }

    /// <summary>
    ///     Gets when this user joined the guild.
    /// </summary>
    /// <returns>
    ///     A <see cref="DateTimeOffset" /> representing the time of which the user has joined the guild;
    ///     <see langword="null" /> when it cannot be obtained.
    /// </returns>
    DateTimeOffset? JoinedAt { get; }


    /// <summary>
    ///     Gets the nickname for this user.
    /// </summary>
    /// <returns>
    ///     A string representing the nickname of the user; <see langword="null" /> if none is set.
    /// </returns>
    string? Nickname { get; }


    /// <summary>
    ///     Gets the guild specific avatar for this user.
    /// </summary>
    /// <returns>
    ///     The users guild avatar hash if they have one; otherwise <see langword="null" />.
    /// </returns>
    string? GuildAvatarId { get; }

    /// <summary>
    ///     Gets the date and time for when this user's guild boost began.
    /// </summary>
    /// <returns>
    ///     A <see cref="DateTimeOffset" /> for when the user began boosting this guild; <see langword="null" /> if they are
    ///     not boosting the guild.
    /// </returns>
    DateTimeOffset? PremiumSince { get; }

    /// <summary>
    ///     Whether the user has passed the guild's Membership Screening requirements.
    /// </summary>
    bool? IsPending { get; }

    /// <summary>
    ///     Gets the date and time that indicates if and for how long a user has been timed out.
    /// </summary>
    /// <remarks>
    ///     <see langword="null" /> or a timestamp in the past if the user is not timed out.
    /// </remarks>
    /// <returns>
    ///     A <see cref="DateTimeOffset" /> indicating how long the user will be timed out for.
    /// </returns>
    DateTimeOffset? TimedOutUntil { get; }

    /// <summary>
    ///     Gets the public flags for this guild member.
    /// </summary>
    GuildMemberFlags Flags { get; }

    ILoadableEntity<ulong, IRole> Role(ulong id) => Roles[id];
}

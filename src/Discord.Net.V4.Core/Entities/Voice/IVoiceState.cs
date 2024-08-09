using Discord.Models;

namespace Discord;

public partial interface IVoiceState :
    IVoiceStateActor,
    ISnowflakeEntity<IVoiceStateModel>
{
    IVoiceChannelActor? Channel { get; }

    DateTimeOffset? RequestToSpeakTimestamp { get; }

    string SessionId { get; }

    /// <summary>
    ///     Gets a value that indicates whether this user is deafened by the guild.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the user is deafened (i.e. not permitted to listen to or speak to others) by the guild;
    ///     otherwise <see langword="false" />.
    /// </returns>
    bool IsDeafened { get; }

    /// <summary>
    ///     Gets a value that indicates whether this user is muted (i.e. not permitted to speak via voice) by the
    ///     guild.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this user is muted by the guild; otherwise <see langword="false" />.
    /// </returns>
    bool IsMuted { get; }

    /// <summary>
    ///     Gets a value that indicates whether this user has marked themselves as deafened.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this user has deafened themselves (i.e. not permitted to listen to or speak to others);
    ///     otherwise <see langword="false" />.
    /// </returns>
    bool IsSelfDeafened { get; }

    /// <summary>
    ///     Gets a value that indicates whether this user has marked themselves as muted (i.e. not permitted to
    ///     speak via voice).
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if this user has muted themselves; otherwise <see langword="false" />.
    /// </returns>
    bool IsSelfMuted { get; }

    /// <summary>
    ///     Gets a value that indicates whether the user is muted by the current user.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the guild is temporarily blocking audio to/from this user; otherwise
    ///     <see langword="false" />.
    /// </returns>
    bool IsSuppressed { get; }

    /// <summary>
    ///     Gets a value that indicates if this user is streaming in a voice channel.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the user is streaming; otherwise <see langword="false" />.
    /// </returns>
    bool IsStreaming { get; }

    /// <summary>
    ///     Gets a value that indicates if the user is videoing in a voice channel.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the user has their camera turned on; otherwise <see langword="false" />.
    /// </returns>
    bool IsVideoing { get; }
}

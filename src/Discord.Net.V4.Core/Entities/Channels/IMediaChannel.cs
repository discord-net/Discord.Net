using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public partial interface IMediaChannel :
    ISnowflakeEntity<IGuildMediaChannelModel>,
    IThreadableChannel,
    IIntegrationChannel,
    IMediaChannelActor
{
    [SourceOfTruth]
    new IGuildMediaChannelModel GetModel();

    /// <summary>
    ///     Gets a value that indicates whether the channel is NSFW.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the channel has the NSFW flag enabled; otherwise <see langword="false" />.
    /// </returns>
    bool IsNsfw { get; }

    /// <summary>
    ///     Gets the current topic for this text channel.
    /// </summary>
    /// <returns>
    ///     A string representing the topic set in the channel; <see langword="null" /> if none is set.
    /// </returns>
    string? Topic { get; }

    /// <summary>
    ///     Gets the default archive duration for a newly created post.
    /// </summary>
    ThreadArchiveDuration DefaultAutoArchiveDuration { get; }

    /// <summary>
    ///     Gets a collection of tags inside of this forum channel.
    /// </summary>
    IReadOnlyCollection<ForumTag> AvailableTags { get; }

    /// <summary>
    ///     Gets the current rate limit on creating posts in this forum channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <see langword="null" /> if disabled.
    /// </returns>
    int? ThreadCreationSlowmode { get; }

    /// <summary>
    ///     Gets the emoji to show in the add reaction button on a thread in a forum channel
    /// </summary>
    ILoadableEntity<IEmote> DefaultReactionEmoji { get; }

    /// <summary>
    ///     Gets the rule used to order posts in forum channels.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see langword="null" />, which indicates a preferred sort order hasn't been set
    /// </remarks>
    SortOrder? DefaultSortOrder { get; }
}

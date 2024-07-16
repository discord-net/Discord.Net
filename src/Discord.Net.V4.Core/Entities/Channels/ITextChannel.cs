using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic channel in a guild that can send and receive messages.
/// </summary>
public partial interface ITextChannel :
    ISnowflakeEntity<IGuildTextChannelModel>,
    IThreadableChannel,
    IMessageChannel,
    IMentionable,
    INestedChannel,
    IIntegrationChannel,
    ITextChannelActor
{
    [SourceOfTruth]
    new IGuildTextChannelModel GetModel();

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
    ///     Gets the current slow-mode delay for this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    /// </returns>
    int SlowModeInterval { get; }

    string IMentionable.Mention => $"<#{Id}>";
}

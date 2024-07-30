using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildChannels))]
public partial interface IGuildChannel :
    ISnowflakeEntity<IGuildChannelModel>,
    IChannel,
    IGuildChannelActor
{
    string Name { get; }

    [SourceOfTruth]
    new IGuildChannelModel GetModel();

    /// <summary>
    ///     Gets the position of this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the position of this channel in the guild's channel list relative to
    ///     others of the same type.
    /// </returns>
    int Position { get; }

    /// <summary>
    ///     Gets the flags related to this channel.
    /// </summary>
    /// <remarks>
    ///     This value is determined by bitwise OR-ing <see cref="ChannelFlags" /> values together.
    /// </remarks>
    /// <returns>
    ///     A channel's flags, if any is associated.
    /// </returns>
    ChannelFlags Flags { get; }

    /// <summary>
    ///     Gets a collection of permission overwrites for this channel.
    /// </summary>
    /// <returns>
    ///     A collection of overwrites associated with this channel.
    /// </returns>
    IReadOnlyCollection<Overwrite> PermissionOverwrites { get; }
}

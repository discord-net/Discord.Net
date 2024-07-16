using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic direct-message channel.
/// </summary>
public partial interface IDMChannel :
    ISnowflakeEntity<IDMChannelModel>,
    IMessageChannel,
    IUpdatable<IDMChannelModel>,
    IDMChannelActor
{
    [SourceOfTruth]
    new IDMChannelModel GetModel();

    /// <summary>
    ///     Gets the recipient of all messages in this channel.
    /// </summary>
    /// <returns>
    ///     A user object that represents the other user in this channel.
    /// </returns>
    IUserActor Recipient { get; }
}

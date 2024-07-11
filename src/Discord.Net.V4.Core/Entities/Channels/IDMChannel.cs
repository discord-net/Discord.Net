using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic direct-message channel.
/// </summary>
public partial interface IDMChannel :
    IMessageChannel,
    IUpdatable<IDMChannelModel>,
    IDMChannelActor
{
    /// <summary>
    ///     Gets the recipient of all messages in this channel.
    /// </summary>
    /// <returns>
    ///     A user object that represents the other user in this channel.
    /// </returns>
    ILoadableEntity<ulong, IUser> Recipient { get; }

    [SourceOfTruth]
    new IDMChannelModel GetModel();
}

using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic direct-message channel.
/// </summary>
public interface IDMChannel :
    IMessageChannel,
    IUpdatable<IDMChannelModel>
{
    /// <summary>
    ///     Gets the recipient of all messages in this channel.
    /// </summary>
    /// <returns>
    ///     A user object that represents the other user in this channel.
    /// </returns>
    ILoadableEntity<ulong, IUser> Recipient { get; }
}

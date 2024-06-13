using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

interface IGroupChannel : IGroupChannel<IGroupChannel>;

/// <summary>
///     Represents a generic private group channel.
/// </summary>
public interface IGroupChannel<out TChannel> :
    IMessageChannel<TChannel>,
    IAudioChannel<TChannel>,
    IGroupChannelActor<TChannel>
    where TChannel : IGroupChannel<TChannel>
{
    /// <summary>
    ///     Gets the users that can access this channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="IDefinedLoadableEntityEnumerable{TId,TEntity}" /> of users that can access this channel.
    /// </returns>
    IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients { get; }
}

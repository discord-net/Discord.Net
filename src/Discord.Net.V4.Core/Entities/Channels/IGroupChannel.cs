using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic private group channel.
/// </summary>
public partial interface IGroupChannel :
    ISnowflakeEntity<IGroupDMChannelModel>,
    IMessageChannel,
    IAudioChannel,
    IGroupChannelActor
{
    /// <summary>
    ///     Gets the users that can access this channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="IDefinedLoadableEntityEnumerable{TId,TEntity}" /> of users that can access this channel.
    /// </returns>
    IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients { get; }

    [SourceOfTruth]
    new IGroupDMChannelModel GetModel();
}

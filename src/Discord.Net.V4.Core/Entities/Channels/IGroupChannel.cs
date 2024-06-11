using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using Modifiable = IModifiable<ulong, IGroupChannel, ModifyGroupDMProperties, ModifyGroupDmParams>;

/// <summary>
///     Represents a generic private group channel.
/// </summary>
public interface IGroupChannel :
    IMessageChannel,
    IAudioChannel,
    Modifiable
{
    /// <summary>
    ///     Gets the users that can access this channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="IDefinedLoadableEntityEnumerable{TId,TEntity}" /> of users that can access this channel.
    /// </returns>
    IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients { get; }

    static ApiBodyRoute<ModifyGroupDmParams> Modifiable.ModifyRoute(IPathable path, ulong id, ModifyGroupDmParams args)
        => Routes.ModifyChannel(id, args);
}

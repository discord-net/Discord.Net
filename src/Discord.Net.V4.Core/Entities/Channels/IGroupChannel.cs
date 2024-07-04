using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IGroupChannel, ModifyGroupDMProperties, ModifyGroupDmParams, IGroupDMChannelModel>;

/// <summary>
///     Represents a generic private group channel.
/// </summary>
public interface IGroupChannel :
    IMessageChannel,
    IAudioChannel,
    IGroupChannelActor,
    IModifiable
{
    static IApiInOutRoute<ModifyGroupDmParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGroupDmParams args
    ) => Routes.ModifyChannel(id, args);

    /// <summary>
    ///     Gets the users that can access this channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="IDefinedLoadableEntityEnumerable{TId,TEntity}" /> of users that can access this channel.
    /// </returns>
    IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients { get; }

    new IGroupDMChannelModel GetModel();
}

using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using Modifiable = IModifiable<ulong, INestedChannel, ModifyGuildChannelPositionProperties, ModifyGuildChannelPositionsParams>;

/// <summary>
///     Represents a type of guild channel that can be nested within a category.
/// </summary>
public interface INestedChannel :
    IGuildChannel,
    IInvitableChannel,
    Modifiable
{
    /// <summary>
    ///     Gets the parent (category) of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     A <see cref="ILoadableEntity{TId,TEntity}" /> representing the category of this channel;
    ///     <see langword="null" /> if none is set.
    /// </returns>
    ILoadableEntity<ulong, ICategoryChannel>? Category { get; }

    static ApiBodyRoute<ModifyGuildChannelPositionsParams> Modifiable.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelPositionsParams args)
    {
        // inject the channel id into the args
        args.Id = id;

        return Routes.ModifyGuildChannelPositions(path.Require<IGuild>(), args);
    }
}

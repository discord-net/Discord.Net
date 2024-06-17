using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a type of guild channel that can be nested within a category.
/// </summary>
public interface INestedChannel :
    IInvitableChannel
{
    /// <summary>
    ///     Gets the parent (category) of this channel in the guild's channel list.
    /// </summary>
    /// <returns>
    ///     A <see cref="ILoadableEntity{TId,TEntity}" /> representing the category of this channel;
    ///     <see langword="null" /> if none is set.
    /// </returns>
    ILoadableEntity<ulong, ICategoryChannel>? Category { get; }
}
